window.LANDING_CONFIG = {
  previewMode: true,

  // Replace this with the real Steam coming soon URL when the page is live.
  steamUrl: "https://store.steampowered.com/app/0000000/Empire_of_Cards/",

  mailchimp: {
    // Paste the action URL from Mailchimp's embedded signup form.
    // Example:
    // https://yourstudio.us1.list-manage.com/subscribe/post?u=abc123&id=def456
    action: "",

    // Keep EMAIL unless your embed code uses a different field name.
    emailField: "EMAIL",

    // Optional hidden fields for tags or signup source.
    // Replace or remove these based on your Mailchimp audience setup.
    hiddenFields: {
      tags: "empire-of-cards-landing"
    }
  }
};
