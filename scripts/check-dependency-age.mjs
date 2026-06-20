#!/usr/bin/env node
// Fails if any package restored under <root> was published within MAX_DEPENDENCY_AGE_DAYS.
// Run after `dotnet restore` so obj/project.assets.json exists.
import { readFileSync, readdirSync } from 'node:fs';
import { join } from 'node:path';

const root = process.argv[2] ?? '.';
const MAX_AGE_DAYS = Number(process.env.MAX_DEPENDENCY_AGE_DAYS ?? 30);
const cutoff = Date.now() - MAX_AGE_DAYS * 24 * 60 * 60 * 1000;

function findAssetsFiles(dir) {
  const found = [];
  for (const entry of readdirSync(dir, { withFileTypes: true })) {
    const full = join(dir, entry.name);
    if (entry.isDirectory()) found.push(...findAssetsFiles(full));
    else if (entry.name === 'project.assets.json') found.push(full);
  }
  return found;
}

const packages = new Map();
for (const file of findAssetsFiles(root)) {
  const assets = JSON.parse(readFileSync(file, 'utf8'));
  for (const key of Object.keys(assets.libraries ?? {})) {
    const [id, version] = key.split('/');
    if (!packages.has(id)) packages.set(id, new Set());
    packages.get(id).add(version);
  }
}

const failures = [];
for (const [id, versions] of packages) {
  for (const version of versions) {
    const url = `https://api.nuget.org/v3/registration5-gz-semver2/${id.toLowerCase()}/${version.toLowerCase()}.json`;
    const res = await fetch(url);
    if (!res.ok) continue;
    const { published } = await res.json();
    if (published && new Date(published).getTime() > cutoff) {
      failures.push({ id, version, published });
    }
  }
}

if (failures.length > 0) {
  console.error(`NuGet packages published within the last ${MAX_AGE_DAYS} days:`);
  for (const f of failures) console.error(`  ${f.id}@${f.version} - published ${f.published}`);
  process.exit(1);
}

console.log(`All NuGet packages are older than ${MAX_AGE_DAYS} days.`);
