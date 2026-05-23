# Empire of Cards landing page

Static single-page marketing site for email capture and Steam-first positioning.

## Files

- `index.html`: page structure and copy
- `styles.css`: full visual design and responsive layout
- `script.js`: CTA tracking, scroll reveals, Mailchimp submission
- `config.js`: Steam and Mailchimp configuration for production

## Local preview

From the repo root:

```bash
python3 -m http.server 4173
```

Then open:

```text
http://localhost:4173/landing-page/
```

## Production setup

1. Open `landing-page/config.js`.
2. Set `previewMode` to `false`.
3. Replace `steamUrl` with the real Steam coming soon URL.
4. Paste the Mailchimp embedded form `action` URL.
5. Update `hiddenFields` to match your Mailchimp audience fields or tags.

## Mailchimp notes

- The form uses Mailchimp's `post-json` endpoint so the page can stay on-site and still show inline success or error states.
- The checkbox is local consent UI. Enable double opt-in and GDPR fields inside Mailchimp for the actual production flow.
- If you need exact signup-source tracking, create the field or tag in Mailchimp first, then mirror its exact input name in `hiddenFields`.

## Tracking hooks

The page emits CTA and form events in three ways:

- pushes objects into `window.dataLayer`
- dispatches `landing:track` browser events
- calls `window.plausible(...)` automatically if Plausible is present

Current event names:

- `cta_click`
- `form_submit_attempt`
- `form_submit_success`
- `form_submit_error`
- `form_submit_preview`
