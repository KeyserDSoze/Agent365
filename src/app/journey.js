const stages = [
  {
    id: 'understand',
    number: '01',
    route: '/architecture',
    label: { it: 'Capire', en: 'Understand' },
    title: {
      it: 'Capire il control plane',
      en: 'Understand the control plane'
    },
    summary: {
      it: 'Mental model, architettura, Registry e responsabilità dei diversi piani Microsoft.',
      en: 'Mental model, architecture, Registry and the responsibilities of each Microsoft control plane.'
    },
    outcome: {
      it: 'Sai spiegare cos’è Agent 365, dove vive ogni capability e come leggere il tenant.',
      en: 'You can explain what Agent 365 is, where each capability lives and how to read the tenant.'
    },
    docs: {
      it: ['docs/01-foundations.md', 'docs/02-architecture.md', 'docs/03-registry-governance.md'],
      en: ['docs/en/01-foundations.md', 'docs/en/02-architecture.md', 'docs/en/03-registry-governance.md']
    }
  },
  {
    id: 'prepare',
    number: '02',
    route: '/assets',
    label: { it: 'Preparare', en: 'Prepare' },
    title: {
      it: 'Preparare tenant, identità e assessment',
      en: 'Prepare tenant, identity and assessment'
    },
    summary: {
      it: 'Prerequisiti, ruoli, licensing, ownership, identity design e customer discovery.',
      en: 'Prerequisites, roles, licensing, ownership, identity design and customer discovery.'
    },
    outcome: {
      it: 'Hai una baseline verificabile e sai cosa manca prima di costruire o governare.',
      en: 'You have a verifiable baseline and know what is missing before building or governing.'
    },
    docs: {
      it: [
        'examples/checklists/tenant-readiness.md',
        'docs/04-entra-agent-id.md',
        'examples/templates/identity-design-sheet.md',
        'examples/checklists/customer-discovery.md'
      ],
      en: [
        'examples/checklists/tenant-readiness.md',
        'docs/en/04-entra-agent-id.md',
        'examples/templates/identity-design-sheet.md',
        'examples/checklists/customer-discovery.md'
      ]
    }
  },
  {
    id: 'build',
    number: '03',
    route: '/developer',
    label: { it: 'Costruire', en: 'Build' },
    title: {
      it: 'Costruire e integrare agenti',
      en: 'Build and integrate agents'
    },
    summary: {
      it: 'SDK, CLI, custom agent, Foundry Local, golden agent, MCP e integrazione con Agent 365.',
      en: 'SDK, CLI, custom agents, Foundry Local, golden agent, MCP and Agent 365 integration.'
    },
    outcome: {
      it: 'Sai portare un agente reale dal runtime locale a un’integrazione governabile.',
      en: 'You can take a real agent from local runtime to a governable integration.'
    },
    docs: {
      it: [
        'docs/09-sdk-cli.md',
        'docs/15-connected-platforms.md',
        'docs/16-local-model-runtime.md',
        'docs/tutorials/01-onboard-existing-agent.md',
        'docs/tutorials/06-dotnet-golden-agent.md',
        'docs/tutorials/07-foundry-local.md',
        'docs/tutorials/08-local-lab-runner.md',
        'docs/tutorials/13-governed-mcp-server.md'
      ],
      en: [
        'docs/en/09-sdk-cli.md',
        'docs/en/15-connected-platforms.md',
        'docs/en/16-local-model-runtime.md',
        'docs/en/tutorials/01-onboard-existing-agent.md',
        'docs/en/tutorials/06-dotnet-golden-agent.md',
        'docs/en/tutorials/07-foundry-local.md',
        'docs/en/tutorials/08-local-lab-runner.md',
        'docs/en/tutorials/13-governed-mcp-server.md'
      ]
    }
  },
  {
    id: 'govern',
    number: '04',
    route: '/capabilities',
    label: { it: 'Governare', en: 'Govern' },
    title: {
      it: 'Governare identità, dati, sicurezza e tool',
      en: 'Govern identity, data, security and tools'
    },
    summary: {
      it: 'Defender, Purview, tool governance, hunting KQL, approval e least privilege.',
      en: 'Defender, Purview, tool governance, KQL hunting, approvals and least privilege.'
    },
    outcome: {
      it: 'Sai individuare gap, definire policy e produrre evidenza tecnica delle decisioni.',
      en: 'You can find gaps, define policy and produce technical evidence for decisions.'
    },
    docs: {
      it: [
        'docs/05-defender.md',
        'docs/17-kql-agent-operations.md',
        'examples/kql/README.md',
        'examples/kql/01-agent-inventory.kql',
        'examples/kql/02-governance-gaps.kql',
        'examples/kql/03-agent-tools-mcp.kql',
        'docs/06-purview.md',
        'docs/07-tools-mcp.md',
        'examples/templates/tool-risk-register.csv',
        'examples/templates/data-interaction-matrix.csv',
        'docs/tutorials/03-defender-hunting.md',
        'docs/tutorials/04-purview-dlp.md',
        'docs/tutorials/10-tool-governance-runtime.md'
      ],
      en: [
        'docs/en/05-defender.md',
        'docs/en/17-kql-agent-operations.md',
        'examples/kql/README.md',
        'examples/kql/01-agent-inventory.kql',
        'examples/kql/02-governance-gaps.kql',
        'examples/kql/03-agent-tools-mcp.kql',
        'docs/en/06-purview.md',
        'docs/en/07-tools-mcp.md',
        'examples/templates/tool-risk-register.csv',
        'examples/templates/data-interaction-matrix.csv',
        'docs/en/tutorials/03-defender-hunting.md',
        'docs/en/tutorials/04-purview-dlp.md',
        'docs/en/tutorials/10-tool-governance-runtime.md'
      ]
    }
  },
  {
    id: 'operate',
    number: '05',
    route: '/operations',
    label: { it: 'Operare', en: 'Operate' },
    title: {
      it: 'Operare, osservare e rispondere',
      en: 'Operate, observe and respond'
    },
    summary: {
      it: 'Observability, run evidence, dashboard, reliability, incident response e customer delivery.',
      en: 'Observability, run evidence, dashboards, reliability, incident response and customer delivery.'
    },
    outcome: {
      it: 'Sai portare il POC in esercizio con evidence, KPI, runbook e recovery gate.',
      en: 'You can move a POC into operations with evidence, KPIs, runbooks and recovery gates.'
    },
    docs: {
      it: [
        'docs/08-observability.md',
        'docs/10-operating-model.md',
        'docs/tutorials/11-run-evidence-observability.md',
        'docs/tutorials/12-operations-dashboard.md',
        'docs/tutorials/14-reliability-incident-operations.md',
        'examples/checklists/agent-incident-runbook.md',
        'docs/13-customer-delivery.md'
      ],
      en: [
        'docs/en/08-observability.md',
        'docs/en/10-operating-model.md',
        'docs/en/tutorials/11-run-evidence-observability.md',
        'docs/en/tutorials/12-operations-dashboard.md',
        'docs/en/tutorials/14-reliability-incident-operations.md',
        'examples/checklists/agent-incident-runbook.md',
        'docs/en/13-customer-delivery.md'
      ]
    }
  }
]

const routeStage = new Map([
  ['/', 'understand'],
  ['/architecture', 'understand'],
  ['/capabilities', 'govern'],
  ['/assets', 'prepare'],
  ['/academy', 'understand'],
  ['/labs', 'build'],
  ['/developer', 'build'],
  ['/operations', 'operate']
])

const explicitPathStage = new Map()
for (const stage of stages) {
  for (const lang of ['it', 'en']) {
    for (const path of stage.docs[lang]) {
      explicitPathStage.set(path, stage.id)
    }
  }
}

export function getJourneyStages(lang = 'it') {
  return stages.map(stage => ({
    ...stage,
    label: stage.label[lang],
    title: stage.title[lang],
    summary: stage.summary[lang],
    outcome: stage.outcome[lang],
    docs: stage.docs[lang]
  }))
}

export function getJourneyStage(id, lang = 'it') {
  return getJourneyStages(lang).find(stage => stage.id === id) || null
}

export function getStageForDocument(path) {
  if (!path) return null
  if (explicitPathStage.has(path)) return explicitPathStage.get(path)
  if (path.startsWith('samples/')) return 'build'
  if (path.startsWith('examples/kql/')) return 'govern'
  if (path.includes('incident')) return 'operate'
  if (path.includes('identity')) return 'prepare'
  return null
}

export function getStageForLocation(pathname) {
  if (!pathname) return null
  if (routeStage.has(pathname)) return routeStage.get(pathname)

  const marker = '/knowledge/'
  if (pathname.startsWith(marker)) {
    const raw = pathname.slice(marker.length)
    const decoded = raw
      .split('/')
      .filter(Boolean)
      .map(segment => decodeURIComponent(segment))
      .join('/')
    return getStageForDocument(decoded)
  }

  return null
}

export function getCuratedDocumentFlow(lang = 'it') {
  return getJourneyStages(lang).flatMap(stage => stage.docs)
}

export function getDocumentNeighbors(path, lang = 'it') {
  const flow = getCuratedDocumentFlow(lang)
  const index = flow.indexOf(path)

  if (index < 0) return { previous: null, next: null }

  return {
    previous: index > 0 ? flow[index - 1] : null,
    next: index < flow.length - 1 ? flow[index + 1] : null
  }
}

export function getKqlPack(lang = 'it') {
  return {
    title: lang === 'it' ? 'KQL Operations Pack' : 'KQL Operations Pack',
    description: lang === 'it'
      ? 'Tre query starter da usare come sequenza: inventario → gap di governance → tool e MCP.'
      : 'Three starter queries intended as a sequence: inventory → governance gaps → tools and MCP.',
    overview: lang === 'it'
      ? 'docs/17-kql-agent-operations.md'
      : 'docs/en/17-kql-agent-operations.md',
    guide: 'examples/kql/README.md',
    items: [
      'examples/kql/01-agent-inventory.kql',
      'examples/kql/02-governance-gaps.kql',
      'examples/kql/03-agent-tools-mcp.kql'
    ]
  }
}

export function getPackForDocument(path, lang = 'it') {
  if (
    path?.startsWith('examples/kql/') ||
    path === 'docs/17-kql-agent-operations.md' ||
    path === 'docs/en/17-kql-agent-operations.md'
  ) return getKqlPack(lang)
  return null
}
