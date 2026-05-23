#!/usr/bin/env node

import fs from "node:fs/promises";
import path from "node:path";

const MODEL = "imagen-4.0-ultra-generate-001";
const ENDPOINT = `https://generativelanguage.googleapis.com/v1beta/models/${MODEL}:predict`;
const STYLE_SUFFIX =
  "stylized 3D cartoon, premium toon shading, bold readable shapes, vibrant clean colors, soft warm lighting, polished game asset, no text, no watermark";
const NEGATIVE_HINT =
  "Avoid realistic photo style, blur, messy composition, low detail, logos, UI, watermark, horror, grunge.";

const PROMPTS = {
  fast_food: [
    "small neighborhood burger shop exterior, red and yellow branding, friendly storefront, sunny street corner, charming fast food atmosphere",
    "burger counter interior, cashier desk, soda machine, tray area, glowing menu feeling, lively service atmosphere",
    "burger kitchen station, grill, fryer, prep counter, ingredient bins, compact high-speed food production setup, isolated composition",
    "burger delivery pickup shelf, packed paper bags, courier helmet, takeaway focus, minimal background",
    "late night burger restaurant exterior, neon glow, warm interior light, empty sidewalk, cozy urban fast food mood",
    "busy lunch rush burger shop, customers waiting, trays moving, cheerful controlled chaos",
    "upgraded burger branch, bigger storefront, sharper branding, more seating, successful local chain feeling",
    "burger combo display asset, burger fries soda on tray, highly appetizing, isolated card illustration, no background",
    "cleaning and hygiene scene inside burger kitchen, staff sanitizing surfaces, readable management theme",
    "premium signature burger stand asset, branded counter, spotlighted hero presentation, isolated object style, no background",
  ],
  cafe: [
    "cozy specialty cafe exterior, wooden facade, big windows, plants, small terrace, warm neighborhood coffee shop mood",
    "espresso bar interior, coffee machine, grinder, ceramic cups, pastry display, calm premium cafe atmosphere",
    "barista workstation asset, espresso machine, milk pitcher, beans, cups, centered composition, minimal background",
    "cafe seating corner, round table, soft lamp, indoor plant, reading and laptop vibe, isolated composition",
    "rainy evening cafe exterior, glowing windows, warm reflections, intimate urban charm",
    "pastry showcase asset, croissants, cakes, elegant dessert display, isolated clean composition, no background",
    "busy morning cafe rush, queue at the bar, warm staff interaction, premium but pressured atmosphere",
    "upgraded cafe terrace, richer furniture, larger seating area, stronger brand personality",
    "loyal customer scene in a cozy cafe, regulars chatting, welcoming service, soft ambient light",
    "signature coffee and dessert card asset, latte art cup with pastry plate, isolated hero object, no background",
  ],
  market_bakkal: [
    "small neighborhood market exterior, colorful fruit crates, bread stand, local storefront, friendly street atmosphere",
    "compact market interior, shelf aisles, fridges, snacks, daily essentials, readable local grocery layout",
    "fresh produce corner asset, tomatoes, peppers, greens, wooden crates, isolated composition, no background",
    "small grocery cashier counter asset, register, candy shelf, shopping basket, centered card art composition",
    "night-open bakkal exterior, warm fluorescent light, quiet street, dependable neighborhood service mood",
    "whatsapp order and local delivery setup, packed bags, scooter ready, practical community commerce feel",
    "upgraded neighborhood market, cleaner shelves, better fridges, stronger fresh section identity",
    "bread and dairy shelf asset, milk, yogurt, loaves, basic essentials, isolated composition, plain background",
    "market fridge breakdown crisis, worried shopkeeper, melting cold products, readable management tension",
    "loyal neighborhood crowd around a local bakkal, families and elderly customers, strong community bond",
  ],
  giyim_magazasi: [
    "stylish local clothing boutique exterior, elegant window display, mannequins, modern fashion storefront",
    "boutique interior with clothing racks, folded tables, fitting mirror, curated premium local fashion vibe",
    "fashion display mannequin asset, trendy outfit, boutique styling, isolated composition, no background",
    "clothing rack asset with curated garments, neat boutique presentation, centered clean card art, plain background",
    "fitting room corner asset, mirror, curtain, stool, elegant retail detail, isolated composition",
    "seasonal sale event inside a boutique, customers browsing, energetic retail pressure",
    "influencer photo shoot in a clothing store, ring light, phone stand, styled outfits, social media retail mood",
    "upgraded fashion boutique, stronger visual merchandising, premium lighting, more confident brand identity",
    "online order packing station for a boutique, branded boxes, tissue paper, shipping workflow, isolated composition",
    "signature outfit hero asset, mannequin with standout fashion look, isolated object style, no background",
  ],
  tech_app: [
    "small startup app office, glass frontage, glowing monitors, creative software company mood",
    "developer workstation asset, dual monitors, keyboard, sticky notes, app prototype on screen, isolated composition",
    "backend server rack asset, glowing dashboards, compact cloud infrastructure look, centered composition, no background",
    "mvp launch war room, small team around screens, excitement and tension, startup release night",
    "crash spike emergency scene, warning monitors, hotfix mood, stressed but readable startup operations",
    "product design corner asset, wireframes, tablets, sticky notes, playful startup planning area, isolated composition",
    "growth analytics setup, large dashboard screens, app metrics, user acquisition mood",
    "late night startup office, pizza boxes, glowing screens, backend pressure, determined indie tech team vibe",
    "scaled-up app company office, more desks, cleaner branding, stronger infrastructure, successful startup growth fantasy",
    "mobile app icon and dashboard hero asset, floating phone screen with analytics glow, isolated composition, no background",
  ],
};

function parseArgs(argv) {
  const options = {
    outDir: path.resolve(process.cwd(), "Artifacts/generated-imagen-assets"),
    aspectRatio: "3:4",
    imageSize: "1K",
    concurrency: 1,
    sampleCount: 1,
    ventures: Object.keys(PROMPTS),
    limit: null,
  };

  for (const arg of argv) {
    if (arg.startsWith("--outdir=")) {
      options.outDir = path.resolve(process.cwd(), arg.slice("--outdir=".length));
    } else if (arg.startsWith("--venture=")) {
      options.ventures = arg
        .slice("--venture=".length)
        .split(",")
        .map((item) => item.trim())
        .filter(Boolean);
    } else if (arg.startsWith("--limit=")) {
      options.limit = Number.parseInt(arg.slice("--limit=".length), 10);
    } else if (arg.startsWith("--concurrency=")) {
      options.concurrency = Math.max(1, Number.parseInt(arg.slice("--concurrency=".length), 10) || 1);
    } else if (arg.startsWith("--aspect-ratio=")) {
      options.aspectRatio = arg.slice("--aspect-ratio=".length);
    } else if (arg.startsWith("--image-size=")) {
      options.imageSize = arg.slice("--image-size=".length);
    } else if (arg.startsWith("--sample-count=")) {
      options.sampleCount = Math.max(1, Math.min(4, Number.parseInt(arg.slice("--sample-count=".length), 10) || 1));
    }
  }

  return options;
}

function buildPrompt(prompt) {
  return `${prompt}, ${STYLE_SUFFIX}. ${NEGATIVE_HINT}`;
}

function buildJobs(selectedVentures, limit) {
  const jobs = [];
  for (const venture of selectedVentures) {
    const prompts = PROMPTS[venture];
    if (!prompts) {
      throw new Error(`Unknown venture: ${venture}`);
    }

    prompts.forEach((prompt, index) => {
      jobs.push({
        venture,
        index: index + 1,
        prompt,
        fullPrompt: buildPrompt(prompt),
      });
    });
  }

  return limit ? jobs.slice(0, limit) : jobs;
}

async function ensureDir(dirPath) {
  await fs.mkdir(dirPath, { recursive: true });
}

async function generateImage(apiKey, job, options) {
  const response = await fetch(ENDPOINT, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      "x-goog-api-key": apiKey,
    },
    body: JSON.stringify({
      instances: [{ prompt: job.fullPrompt }],
      parameters: {
        sampleCount: options.sampleCount,
        aspectRatio: options.aspectRatio,
        imageSize: options.imageSize,
      },
    }),
  });

  if (!response.ok) {
    const errorText = await response.text();
    throw new Error(`HTTP ${response.status} ${response.statusText}: ${errorText}`);
  }

  const data = await response.json();
  const predictions = Array.isArray(data.predictions) ? data.predictions : [];
  const first = predictions[0];
  const bytes = first?.bytesBase64Encoded;

  if (!bytes) {
    throw new Error(`No image bytes in response: ${JSON.stringify(data).slice(0, 1000)}`);
  }

  return {
    buffer: Buffer.from(bytes, "base64"),
    rawResponse: data,
  };
}

async function runPool(items, concurrency, worker) {
  let cursor = 0;
  const results = [];

  async function next() {
    while (cursor < items.length) {
      const currentIndex = cursor++;
      results[currentIndex] = await worker(items[currentIndex], currentIndex);
    }
  }

  await Promise.all(Array.from({ length: Math.min(concurrency, items.length) }, () => next()));
  return results;
}

async function main() {
  const apiKey = process.env.GOOGLE_AI_API_KEY;
  if (!apiKey) {
    throw new Error("GOOGLE_AI_API_KEY is required.");
  }

  const options = parseArgs(process.argv.slice(2));
  const jobs = buildJobs(options.ventures, options.limit);
  const timestamp = new Date().toISOString().replace(/[:.]/g, "-");
  const sessionDir = path.join(options.outDir, timestamp);

  await ensureDir(sessionDir);

  const manifest = {
    model: MODEL,
    endpoint: ENDPOINT,
    createdAt: new Date().toISOString(),
    options,
    jobs: [],
  };

  console.log(`Generating ${jobs.length} image(s) into ${sessionDir}`);

  await runPool(jobs, options.concurrency, async (job, jobNumber) => {
    const ventureDir = path.join(sessionDir, job.venture);
    await ensureDir(ventureDir);

    console.log(`[${jobNumber + 1}/${jobs.length}] ${job.venture} #${job.index}`);
    const result = await generateImage(apiKey, job, options);
    const fileName = `${String(job.index).padStart(2, "0")}.png`;
    const outputPath = path.join(ventureDir, fileName);

    await fs.writeFile(outputPath, result.buffer);
    manifest.jobs.push({
      venture: job.venture,
      index: job.index,
      outputPath,
      prompt: job.prompt,
      fullPrompt: job.fullPrompt,
    });
  });

  const manifestPath = path.join(sessionDir, "manifest.json");
  await fs.writeFile(manifestPath, JSON.stringify(manifest, null, 2));

  console.log(`Done. Manifest: ${manifestPath}`);
}

main().catch((error) => {
  console.error(error.message);
  process.exitCode = 1;
});
