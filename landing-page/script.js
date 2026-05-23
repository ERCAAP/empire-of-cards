(function landingPage() {
  const defaults = {
    previewMode: true,
    steamUrl: "",
    steamPlaceholderUrl: "https://store.steampowered.com/app/0000000/Empire_of_Cards/",
    mailchimp: {
      action: "",
      emailField: "EMAIL",
      hiddenFields: {
        tags: "empire-of-cards-landing"
      }
    }
  };

  const config = mergeConfig(defaults, window.LANDING_CONFIG || {});

  const setupBanner = document.querySelector("[data-setup-banner]");
  const steamLinks = [...document.querySelectorAll("[data-steam-link]")];
  const form = document.querySelector("[data-mailchimp-form]");
  const message = document.querySelector("[data-form-message]");
  const honeypot = document.querySelector("[data-honeypot]");
  const reveals = [...document.querySelectorAll(".reveal")];

  applySteamLinks();
  configureMailchimpHoneypot();
  setupRevealObserver();
  bindTracking();
  bindForm();
  toggleSetupBanner();

  function applySteamLinks() {
    const targetUrl = isConfiguredSteamUrl() ? config.steamUrl : "#updates";

    steamLinks.forEach((link) => {
      link.setAttribute("href", targetUrl);

      if (!isConfiguredSteamUrl()) {
        link.setAttribute("aria-disabled", "true");
        link.classList.add("is-disabled");
      } else {
        link.removeAttribute("aria-disabled");
        link.classList.remove("is-disabled");
      }
    });
  }

  function isConfiguredSteamUrl() {
    return Boolean(
      config.steamUrl &&
        config.steamUrl !== defaults.steamPlaceholderUrl &&
        /^https?:\/\//.test(config.steamUrl)
    );
  }

  function configureMailchimpHoneypot() {
    if (!honeypot) return;

    const action = config.mailchimp.action;
    if (!action) {
      honeypot.name = "website";
      return;
    }

    try {
      const url = new URL(action);
      const u = url.searchParams.get("u");
      const id = url.searchParams.get("id");
      honeypot.name = u && id ? `b_${u}_${id}` : "website";
    } catch {
      honeypot.name = "website";
    }
  }

  function bindTracking() {
    document.addEventListener("click", (event) => {
      const trigger = event.target.closest("[data-track]");
      if (!trigger) return;

      track("cta_click", {
        cta: trigger.getAttribute("data-track")
      });
    });
  }

  function bindForm() {
    if (!form) return;

    form.addEventListener("submit", async (event) => {
      event.preventDefault();

      const emailInput = form.querySelector('input[type="email"]');
      const consentInput = form.querySelector('input[type="checkbox"]');
      const submitButton = form.querySelector('button[type="submit"]');

      clearMessage();

      if (!emailInput.checkValidity()) {
        showError("Enter a valid email address.");
        emailInput.focus();
        return;
      }

      if (!consentInput.checked) {
        showError("Please confirm you want dev updates and launch news.");
        consentInput.focus();
        return;
      }

      if (!config.mailchimp.action) {
        if (config.previewMode) {
          submitButton.disabled = true;
          showPending("Preview mode: simulating signup...");
          await wait(650);
          submitButton.disabled = false;
          emailInput.value = "";
          consentInput.checked = false;
          showSuccess(
            "You’re in. First updates will include gameplay progress and Steam page news. Preview mode is active, so no email was actually submitted yet."
          );
          track("form_submit_preview", {
            source: "landing-page"
          });
          return;
        }

        showError("Mailchimp is not configured yet. Add the action URL in config.js.");
        return;
      }

      submitButton.disabled = true;
      showPending("Sending...");

      const result = await submitToMailchimp({
        email: emailInput.value.trim(),
        consent: consentInput.checked
      });

      submitButton.disabled = false;

      if (result.ok) {
        emailInput.value = "";
        consentInput.checked = false;
        showSuccess(
          "You’re in. First updates will include gameplay progress and Steam page news. Check your inbox for the confirmation email."
        );
        track("form_submit_success", {
          source: "landing-page"
        });
      } else {
        showError(result.message);
        track("form_submit_error", {
          source: "landing-page"
        });
      }
    });
  }

  function submitToMailchimp({ email }) {
    return new Promise((resolve) => {
      const action = config.mailchimp.action;
      let url;

      try {
        url = new URL(action);
      } catch {
        resolve({
          ok: false,
          message: "Mailchimp action URL is invalid. Check config.js."
        });
        return;
      }

      const callbackName = `mailchimpCallback_${Date.now()}`;
      const jsonpUrl = new URL(action.replace("/post?", "/post-json?"));
      const formFields = new URLSearchParams(url.search);

      formFields.set(config.mailchimp.emailField || "EMAIL", email);
      formFields.set("c", callbackName);

      Object.entries(config.mailchimp.hiddenFields || {}).forEach(([key, value]) => {
        if (value !== undefined && value !== null && value !== "") {
          formFields.set(key, value);
        }
      });

      const script = document.createElement("script");
      script.src = `${jsonpUrl.origin}${jsonpUrl.pathname}?${formFields.toString()}`;
      script.async = true;

      let settled = false;
      const cleanup = () => {
        delete window[callbackName];
        script.remove();
      };

      const finalize = (payload) => {
        if (settled) return;
        settled = true;
        cleanup();
        resolve(payload);
      };

      window[callbackName] = (response) => {
        const responseText = decodeMailchimpMessage(response.msg || "");
        const alreadySubscribed = /already subscribed/i.test(responseText);

        if (response.result === "success" || alreadySubscribed) {
          finalize({
            ok: true
          });
          return;
        }

        finalize({
          ok: false,
          message: responseText || "Something went wrong while joining the list."
        });
      };

      script.onerror = () => {
        finalize({
          ok: false,
          message: "The signup request could not be completed. Check the Mailchimp action URL."
        });
      };

      document.body.appendChild(script);
      track("form_submit_attempt", {
        source: "landing-page"
      });
    });
  }

  function decodeMailchimpMessage(message) {
    const temp = document.createElement("div");
    temp.innerHTML = message;
    return temp.textContent ? temp.textContent.trim() : "";
  }

  function setupRevealObserver() {
    if (!("IntersectionObserver" in window)) {
      reveals.forEach((item) => item.classList.add("is-visible"));
      return;
    }

    const observer = new IntersectionObserver(
      (entries) => {
        entries.forEach((entry) => {
          if (entry.isIntersecting) {
            entry.target.classList.add("is-visible");
            observer.unobserve(entry.target);
          }
        });
      },
      {
        threshold: 0.18
      }
    );

    reveals.forEach((item) => observer.observe(item));
  }

  function showPending(text) {
    if (!message) return;
    message.textContent = text;
    message.classList.remove("is-success", "is-error");
  }

  function showSuccess(text) {
    if (!message) return;
    message.textContent = text;
    message.classList.remove("is-error");
    message.classList.add("is-success");
  }

  function showError(text) {
    if (!message) return;
    message.textContent = text;
    message.classList.remove("is-success");
    message.classList.add("is-error");
  }

  function clearMessage() {
    if (!message) return;
    message.textContent = "";
    message.classList.remove("is-success", "is-error");
  }

  function toggleSetupBanner() {
    if (!setupBanner) return;

    const needsSetup = config.previewMode || !config.mailchimp.action || !isConfiguredSteamUrl();
    setupBanner.hidden = !needsSetup;
  }

  function track(name, properties = {}) {
    const payload = { event: name, ...properties };
    window.dataLayer = window.dataLayer || [];
    window.dataLayer.push(payload);
    document.dispatchEvent(new CustomEvent("landing:track", { detail: payload }));

    if (typeof window.plausible === "function") {
      window.plausible(name, { props: properties });
    }
  }

  function wait(milliseconds) {
    return new Promise((resolve) => window.setTimeout(resolve, milliseconds));
  }

  function mergeConfig(base, override) {
    const output = { ...base };

    Object.keys(override || {}).forEach((key) => {
      const baseValue = output[key];
      const overrideValue = override[key];

      if (isObject(baseValue) && isObject(overrideValue)) {
        output[key] = mergeConfig(baseValue, overrideValue);
      } else {
        output[key] = overrideValue;
      }
    });

    return output;
  }

  function isObject(value) {
    return Boolean(value) && typeof value === "object" && !Array.isArray(value);
  }
})();
