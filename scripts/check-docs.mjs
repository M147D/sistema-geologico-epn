#!/usr/bin/env node
// Verifica la documentación codificada del proyecto (índice en CLAUDE.md, "Documentation Index"):
// 1. (opcional) ninguna referencia a rutas viejas, pasadas con --ruta-vieja=<texto>
//    (repetible), fuera de los specs/planes históricos de docs/superpowers/;
// 2. todo link relativo de la documentación indexada apunta a un archivo existente;
// 3. todo código (ARQ-, DEP-, PS-, LA-, SK-, PEN-, ART-, DIA-, REV-, CAM-) citado está definido en exactamente un
//    título, todo código definido aparece en CLAUDE.md, y todo SPEC-/PLAN-<fecha> citado
//    tiene su archivo en docs/superpowers/.
// 4. cada revisión de diseño REV-NN cita un commit real, y su evidencia existe en ese commit.
// 5. cada cambio CAM-NN tiene sus datos, y sus commits existen.
// Uso: node scripts/check-docs.mjs [--ruta-vieja=docs-viejos/ ...]
import { execFileSync } from 'node:child_process';
import { existsSync, readFileSync, readdirSync, statSync } from 'node:fs';
import { dirname, join, relative, resolve } from 'node:path';
import { fileURLToPath } from 'node:url';

const ROOT = resolve(dirname(fileURLToPath(import.meta.url)), '..');
const IGNORAR = new Set(['.git', 'node_modules', 'vendor', 'dist', 'build', 'deploy', '.astro', '.next']);
const ESTE_ARCHIVO = rel(fileURLToPath(import.meta.url));
const RUTAS_VIEJAS = process.argv.slice(2)
  .filter((a) => a.startsWith('--ruta-vieja='))
  .map((a) => a.slice('--ruta-vieja='.length))
  .filter(Boolean);
const errores = [];

function rel(p) {
  return relative(ROOT, p).split('\\').join('/');
}

function listar(dir, out = []) {
  for (const nombre of readdirSync(dir)) {
    if (IGNORAR.has(nombre)) continue;
    const ruta = join(dir, nombre);
    if (statSync(ruta).isDirectory()) {
      // Otro checkout dentro del repo (p. ej. .claude/worktrees/*): no es documentación de este árbol.
      if (existsSync(join(ruta, '.git'))) continue;
      listar(ruta, out);
    } else out.push(ruta);
  }
  return out;
}

const INDICE_CANDIDATOS = ['GEMINI.md', 'AGENTS.md', 'CLAUDE.md'];
const indicesPresentes = INDICE_CANDIDATOS.filter((n) => existsSync(join(ROOT, n)));
if (indicesPresentes.length === 0) {
  console.error('check-docs: falta GEMINI.md, AGENTS.md o CLAUDE.md en la raíz del repo (lleva el "Documentation Index").');
  process.exit(1);
}

const archivos = listar(ROOT);
const esHistorico = (p) => rel(p).startsWith('docs/superpowers/');
const esTexto = (p) => /\.(md|mdx|version\d+|mjs|cjs|js|jsx|ts|tsx|astro|vue|php|py|json|css|txt|example|ya?ml)$/.test(p);

// 1. Rutas viejas (solo si se pidieron)
for (const vieja of RUTAS_VIEJAS) {
  for (const archivo of archivos) {
    if (!esTexto(archivo) || esHistorico(archivo) || rel(archivo) === ESTE_ARCHIVO) continue;
    readFileSync(archivo, 'utf8').split('\n').forEach((linea, i) => {
      if (linea.includes(vieja)) errores.push(`${rel(archivo)}:${i + 1}: referencia a la ruta vieja ${vieja}`);
    });
  }
}

// Documentación indexada: GEMINI.md, AGENTS.md, CLAUDE.md, README.md, las skills del proyecto y docs/ salvo superpowers/.
// documenting-projects se excluye: sus códigos son ejemplos, no documentos de este proyecto.
const DOCS = archivos.filter((p) => {
  const r = rel(p);
  if (esHistorico(p) || r.includes('/skills/documenting-projects/')) return false;
  return ['GEMINI.md', 'AGENTS.md', 'CLAUDE.md', 'README.md'].includes(r)
    || /^\.(agents|claude)\/skills\/[^/]+\/SKILL\.md$/.test(r)
    || /^skills\/[^/]+\/SKILL\.md$/.test(r)
    || (r.startsWith('docs/') && /\.md(\.version\d+)?$/.test(r));
});

const sinCodigo = (texto) => texto
  .replace(/^[ \t]*```[\s\S]*?^[ \t]*```/gm, '')
  .replace(/`[^`\n]*`/g, '');

// 2. Links relativos
const LINK = /\[[^\]]*\]\(([^)\s]+)\)/g;
for (const doc of DOCS) {
  const texto = sinCodigo(readFileSync(doc, 'utf8'));
  for (const [, destino] of texto.matchAll(LINK)) {
    if (/^(https?:|mailto:|#)/.test(destino)) continue;
    const ruta = resolve(dirname(doc), decodeURI(destino.split('#')[0]));
    if (!existsSync(ruta)) errores.push(`${rel(doc)}: link roto -> ${destino}`);
  }
}

// 3. Bloques Mermaid: no vacíos y con un tipo de diagrama que GitHub sabe dibujar
const TIPOS_MERMAID = /^(flowchart|graph|sequenceDiagram|stateDiagram(-v2)?|classDiagram|erDiagram|journey|gantt|pie|quadrantChart|requirementDiagram|gitGraph|C4Context|C4Container|C4Component|C4Dynamic|C4Deployment|mindmap|timeline|sankey-beta|xychart-beta|block-beta|packet-beta|kanban|architecture-beta|radar-beta)\b/;
for (const doc of DOCS) {
  const contenido = readFileSync(doc, 'utf8');
  const bloquesMermaid = contenido.matchAll(/^[ \t]*```mermaid[ \t]*\r?\n([\s\S]*?)^[ \t]*```/gm);
  for (const match of bloquesMermaid) {
    const lineas = match[1].trim().split('\n').map((l) => l.trim()).filter((l) => l && !l.startsWith('%%'));
    if (lineas.length === 0) {
      errores.push(`${rel(doc)}: bloque mermaid vacío`);
    } else if (!TIPOS_MERMAID.test(lineas[0])) {
      errores.push(`${rel(doc)}: bloque mermaid con tipo de diagrama no soportado o inválido: "${lineas[0]}"`);
    }
  }
}

// 4. Códigos
const COD = String.raw`(?:ARQ-(?:\d{2}|H\d)|DEP-\d{2}|PS-\d{2}|LA-\d{2}|SK-\d{2}|PEN-\d{2}|ART-\d{2}|DIA-\d{2}|REV-\d{2}|CAM-\d{2})`;
const TITULO = new RegExp(String.raw`^#{1,2} (${COD}) · `, 'gm');
const CITA = new RegExp(String.raw`\b${COD}\b`, 'g');

const definidos = new Map();
for (const doc of DOCS.filter((p) => rel(p).startsWith('docs/'))) {
  for (const [, codigo] of readFileSync(doc, 'utf8').matchAll(TITULO)) {
    definidos.set(codigo, [...(definidos.get(codigo) ?? []), rel(doc)]);
  }
}
for (const [codigo, donde] of definidos) {
  if (donde.length > 1) errores.push(`${codigo}: definido en más de un título (${donde.join(', ')})`);
}

const textoIndices = indicesPresentes.map((n) => readFileSync(join(ROOT, n), 'utf8')).join('\n');
for (const codigo of definidos.keys()) {
  if (!new RegExp(String.raw`\b${codigo}\b`).test(textoIndices)) {
    errores.push(`${codigo}: no aparece en el índice de ${indicesPresentes.join(' ni ')}`);
  }
}

for (const doc of DOCS) {
  const texto = readFileSync(doc, 'utf8');
  for (const [codigo] of texto.matchAll(CITA)) {
    if (!definidos.has(codigo)) errores.push(`${rel(doc)}: cita ${codigo}, que no está definido en ningún título (si el documento aún no existe, nómbralo sin su código)`);
  }
  for (const [, tipo, fecha] of texto.matchAll(/\b(SPEC|PLAN)-(\d{4}-\d{2}-\d{2})\b/g)) {
    const carpeta = join(ROOT, 'docs/superpowers', tipo === 'SPEC' ? 'specs' : 'plans');
    if (!existsSync(carpeta) || !readdirSync(carpeta).some((n) => n.startsWith(fecha))) {
      errores.push(`${rel(doc)}: cita ${tipo}-${fecha}, sin archivo en ${rel(carpeta)}/`);
    }
  }
}

// 5. Artefactos: cada archivo de scripts/artefactos/ se llama ART-NN-<slug>.<ext> con un
//    código que tiene su ficha "## ART-NN · …" en docs/, y cada ficha tiene su archivo.
const carpetaArt = join(ROOT, 'scripts', 'artefactos');
const archivosArt = existsSync(carpetaArt) ? readdirSync(carpetaArt).filter((n) => !n.startsWith('.')) : [];
for (const nombre of archivosArt) {
  const m = nombre.match(/^(ART-\d{2})-/);
  if (!m) errores.push(`scripts/artefactos/${nombre}: el nombre debe empezar con su código (ART-NN-<slug>.<ext>)`);
  else if (!definidos.has(m[1])) errores.push(`scripts/artefactos/${nombre}: ${m[1]} no tiene ficha "## ${m[1]} · …" en docs/`);
}
for (const codigo of definidos.keys()) {
  if (codigo.startsWith('ART-') && !archivosArt.some((n) => n.startsWith(`${codigo}-`))) {
    errores.push(`${codigo}: falta su archivo scripts/artefactos/${codigo}-<slug>.<ext>`);
  }
}

// 6. Diagramas: cada DIA-NN salvo DIA-00 tiene al menos un bloque mermaid, y el
//    índice DIA-00 cita todos los DIA definidos.
const docsDia = [...definidos].filter(([codigo]) => codigo.startsWith('DIA-'));
for (const [codigo, donde] of docsDia) {
  if (codigo === 'DIA-00') continue;
  if (!/^```mermaid\s*$/m.test(readFileSync(join(ROOT, donde[0]), 'utf8'))) {
    errores.push(`${codigo}: ${donde[0]} no tiene ningún diagrama mermaid`);
  }
}
const indiceDia = docsDia.find(([codigo]) => codigo === 'DIA-00');
if (docsDia.length && !indiceDia) errores.push('hay documentos DIA pero falta el índice DIA-00');
if (indiceDia) {
  const textoIndice = readFileSync(join(ROOT, indiceDia[1][0]), 'utf8');
  for (const [codigo] of docsDia) {
    if (codigo !== 'DIA-00' && !new RegExp(String.raw`\b${codigo}\b`).test(textoIndice)) {
      errores.push(`DIA-00 no cita ${codigo}`);
    }
  }
}

// 7. Revisiones de diseño: la evidencia se comprueba en el commit revisado, no en el árbol
//    actual, porque una revisión es una foto fechada y el código sigue cambiando.
function lineasEnCommit(commit, ruta) {
  try {
    const texto = execFileSync('git', ['-C', ROOT, 'show', `${commit}:${ruta}`], {
      encoding: 'utf8', stdio: ['ignore', 'pipe', 'ignore'], maxBuffer: 64 * 1024 * 1024,
    });
    return texto.replace(/\n$/, '').split('\n').length;
  } catch {
    return null;
  }
}

const EVIDENCIA = /`([^`\n]+):(\d+)(?:-(\d+))?`/g;
for (const [codigo, donde] of definidos) {
  if (!codigo.startsWith('REV-')) continue;
  const contenido = readFileSync(join(ROOT, donde[0]), 'utf8');
  const commit = contenido.match(/^\*\*Commit revisado:\*\*\s*`?([0-9a-f]{7,40})`?/m)?.[1];
  if (!commit) {
    errores.push(`${codigo}: falta "**Commit revisado:** <hash>"`);
    continue;
  }
  try {
    execFileSync('git', ['-C', ROOT, 'cat-file', '-e', `${commit}^{commit}`], { stdio: 'ignore' });
  } catch {
    errores.push(`${codigo}: el commit revisado ${commit} no existe en este repo`);
    continue;
  }
  for (const seccion of ['## Hallazgos', '## Descartados']) {
    if (!contenido.split('\n').some((l) => l.trim() === seccion)) errores.push(`${codigo}: falta la sección "${seccion}"`);
  }
  const vistos = new Set();
  for (const linea of contenido.split('\n')) {
    const id = linea.match(/^\|\s*([HP]-\d{2})\s*\|/)?.[1];
    if (!id) continue;
    if (vistos.has(id)) errores.push(`${codigo}: ${id} repetido`);
    vistos.add(id);
    const celdas = linea.split(/(?<!\\)\|/).map((c) => c.trim());
    const citas = [...(celdas[5] ?? '').matchAll(EVIDENCIA)];
    if (citas.length === 0) errores.push(`${codigo}: ${id} no cita evidencia \`archivo:línea\` en su columna Evidencia`);
    for (const [, ruta, inicio, fin] of citas) {
      const total = lineasEnCommit(commit, ruta);
      if (total === null) errores.push(`${codigo}: ${id} cita ${ruta}, que no existe en ${commit}`);
      else if (Number(fin ?? inicio) > total) errores.push(`${codigo}: ${id} cita ${ruta}:${fin ?? inicio}, pero en ${commit} tiene ${total} líneas`);
    }
    if (id.startsWith('H-')) {
      const severidad = celdas[2]?.toLowerCase();
      if (!['alta', 'media', 'baja'].includes(severidad)) errores.push(`${codigo}: ${id} tiene una severidad inválida ("${celdas[2]}")`);
      const seguimiento = celdas[celdas.length - 2] ?? '';
      if (severidad === 'alta' && !/PEN-\d{2}|aceptado/i.test(seguimiento)) {
        errores.push(`${codigo}: ${id} es de severidad alta y no enlaza un PEN ni dice "Aceptado"`);
      }
    }
  }
  if (![...vistos].some((id) => id.startsWith('H-')) && !/sin hallazgos/i.test(contenido)) {
    errores.push(`${codigo}: no tiene filas H-NN ni dice "Sin hallazgos"`);
  }
}

// 8. Control de cambios: cada entrada dice de qué commit salió y si llegó a producción.
const TIPOS_CAMBIO = ['despliegue', 'versión', 'migración', 'arquitectura', 'revisión'];
const ESTADOS_PRODUCCION = /^\*\*Producción:\*\*\s*(sí|no aplica|no|desconocido)(?!\p{L})/imu;
const archivosCam = new Set([...definidos].filter(([codigo]) => codigo.startsWith('CAM-')).map(([, donde]) => donde[0]));
for (const archivo of archivosCam) {
  const entradas = readFileSync(join(ROOT, archivo), 'utf8').split(/^(?=## CAM-\d{2} · )/m).filter((e) => e.startsWith('## CAM-'));
  for (const entrada of entradas) {
    const codigo = entrada.slice(3, 9);
    if (!/^\*\*Fecha:\*\*\s*\d{4}-\d{2}-\d{2}\b/m.test(entrada)) errores.push(`${codigo}: falta "**Fecha:** AAAA-MM-DD"`);
    const tipo = entrada.match(/^\*\*Tipo:\*\*\s*(\S+?)[.\s]/m)?.[1]?.toLowerCase();
    if (!TIPOS_CAMBIO.includes(tipo)) errores.push(`${codigo}: "**Tipo:**" debe ser ${TIPOS_CAMBIO.join(', ')}`);
    if (!ESTADOS_PRODUCCION.test(entrada)) errores.push(`${codigo}: "**Producción:**" debe empezar con sí, no, desconocido o no aplica`);
    const hashes = [...(entrada.match(/^\*\*Commit:\*\*(.*)$/m)?.[1] ?? '').matchAll(/`([0-9a-f]{7,40})`/g)].map((m) => m[1]);
    if (hashes.length === 0) errores.push(`${codigo}: falta "**Commit:**" con al menos un hash`);
    for (const hash of hashes) {
      try {
        execFileSync('git', ['-C', ROOT, 'cat-file', '-e', `${hash}^{commit}`], { stdio: 'ignore' });
      } catch {
        errores.push(`${codigo}: el commit ${hash} no existe en este repo`);
      }
    }
  }
}

if (errores.length) {
  console.error(`check-docs: ${errores.length} problema(s)\n${errores.map((e) => `  - ${e}`).join('\n')}`);
  process.exit(1);
}
console.log(`check-docs: OK — ${DOCS.length} documentos, ${definidos.size} códigos definidos en ${indicesPresentes.join(', ')}.`);

