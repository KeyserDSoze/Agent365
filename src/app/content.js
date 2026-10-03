export const officialLinks = [
  { label: 'Agent 365 documentation', url: 'https://learn.microsoft.com/en-us/microsoft-agent-365/' },
  { label: 'Agent 365 overview', url: 'https://learn.microsoft.com/en-us/microsoft-agent-365/overview' },
  { label: 'Agent management in Microsoft 365 admin center', url: 'https://learn.microsoft.com/en-us/microsoft-365/admin/manage/agent-365-overview?view=o365-worldwide' },
  { label: 'Microsoft Entra Agent ID', url: 'https://learn.microsoft.com/en-us/entra/agent-id/' },
  { label: 'Secure AI agents with Agent 365', url: 'https://learn.microsoft.com/en-us/security/security-for-ai/agent-365-security' },
  { label: 'Agent 365 CLI', url: 'https://learn.microsoft.com/en-us/microsoft-agent-365/developer/agent-365-cli' },
  { label: 'Agent 365 learning path', url: 'https://learn.microsoft.com/en-us/training/paths/agent-365-solutions/' }
]

export const copy = {
  it: {
    eyebrow: 'KNOWLEDGE HUB · AGGIORNATO A OTTOBRE 2026',
    heroTitle: 'Microsoft Agent 365, dalla teoria alla delivery.',
    heroBody: 'Una base tecnica bilingue per capire, progettare, governare e mettere in sicurezza gli agenti AI con Microsoft 365, Entra, Defender e Purview. Con esempi, checklist, laboratori e un percorso di skilling completo.',
    primaryCta: 'Esplora l’architettura',
    secondaryCta: 'Piano di formazione',
    search: 'Cerca un tema…',
    status: 'Stato del prodotto',
    ga: 'Commercial GA dal 1° maggio 2026',
    principle: 'Principio guida',
    principleText: 'Agent 365 è un control plane trasversale: non “vive dentro Defender”. Microsoft 365 admin center, Entra, Defender e Purview cooperano su piani diversi.',
    nav: {
      overview: 'Start', journey: 'Percorso', architecture: 'Architettura', domains: 'Domini', examples: 'Esempi', training: 'Academy', labs: 'Lab', developer: 'Developer', operations: 'Operations', knowledge: 'Knowledge', sources: 'Fonti'
    },
    overviewTitle: 'La mappa mentale',
    overviewIntro: 'Per lavorare bene su Agent 365 bisogna distinguere il control plane dai runtime degli agenti. Il valore è portare inventory, identity, access, data protection, threat protection e observability dentro un modello operativo coerente.',
    pillars: [
      ['Observe', 'Inventario, Agent Registry, Agent Map, utilizzo, attività e segnali di rischio.'],
      ['Govern', 'Ownership, lifecycle, approval, policy, tool governance e responsabilità.'],
      ['Secure', 'Identità, least privilege, Conditional Access, posture, threat detection e data protection.'],
      ['Extend', 'SDK, CLI, connected platforms, custom agents, MCP e integrazioni.'],
      ['Operate', 'Runbook, incident response, KPI, RACI, assessment, POC e miglioramento continuo.']
    ],
    architectureTitle: 'Architettura logica',
    architectureIntro: 'Agent 365 mette in relazione sistemi che esistevano già con capability specifiche per gli agenti. Ogni piano ha un ruolo preciso.',
    layers: [
      { n:'01', title:'Management plane', product:'Microsoft 365 admin center', text:'Agent overview, Agent Registry, richieste, owner, amministrazione e visibilità tenant-wide.' },
      { n:'02', title:'Identity plane', product:'Microsoft Entra Agent ID', text:'Agent identity blueprint, agent identities, sponsor, autenticazione, autorizzazione, Conditional Access e governance.' },
      { n:'03', title:'Security plane', product:'Microsoft Defender', text:'Posture, Advanced Hunting, AgentsInfo, rilevamento minacce, investigazione e runtime protection.' },
      { n:'04', title:'Data plane', product:'Microsoft Purview', text:'Audit, DLP, sensitivity labels, eDiscovery, data security, compliance e controllo delle interazioni con i dati.' },
      { n:'05', title:'Tool & integration plane', product:'Agent 365 SDK / CLI / MCP', text:'Onboarding, tool registry, connected platforms, OpenTelemetry e agenti Microsoft o terze parti.' }
    ],
    domainsTitle: 'Domini di competenza',
    domainsIntro: 'Questi sono i blocchi che una risorsa tecnica deve saper spiegare, configurare e verificare.',
    domains: [
      { id:'registry', tag:'Observe', title:'Agent Registry & Inventory', summary:'La source of truth degli agenti presenti nel tenant.', bullets:['Inventario tenant-wide e agent discovery','Ownership, stato e metadata','Agent Map e relazioni','Connected platforms e shadow agents'], example:'Output pratico: inventory baseline con owner, piattaforma, identità, strumenti, dati e rischio.' },
      { id:'identity', tag:'Secure', title:'Microsoft Entra Agent ID', summary:'Identità purpose-built per agenti interattivi e autonomi.', bullets:['Agent identity blueprint','Agent identities e sponsor','OAuth, token e autorizzazione','Conditional Access e Identity Protection'], example:'Output pratico: Identity Design Sheet con trust boundary, credential model, resource access e least privilege.' },
      { id:'defender', tag:'Secure', title:'Microsoft Defender', summary:'Posture, hunting, detection e risposta per gli agenti.', bullets:['AgentsInfo in Advanced Hunting','Rischio e posture','Threat detection e incident workflow','Runtime protection e investigazione'], example:'Output pratico: query pack KQL + incident playbook dedicato agli agenti.' },
      { id:'purview', tag:'Govern', title:'Microsoft Purview', summary:'Protezione dei dati, audit e compliance nelle interazioni agentiche.', bullets:['Audit e tracciabilità','DLP e sensitivity labels','DSPM / data security','eDiscovery e compliance'], example:'Output pratico: Data Interaction Matrix agente → dati → azioni → policy → evidenza.' },
      { id:'tools', tag:'Govern', title:'Tools & MCP Governance', summary:'Controllare quali strumenti un agente può scoprire e usare.', bullets:['Tool Registry e catalogo','MCP server e connettori','Risk tier dei tool','Approval, least privilege e monitoraggio'], example:'Output pratico: Tool Risk Register con owner, scope, permessi, impatto e mitigazioni.' },
      { id:'observability', tag:'Observe', title:'Observability', summary:'Capire attività, prestazioni e comportamento degli agenti.', bullets:['Activity e usage signals','OpenTelemetry','Log e retention','Correlation con identity e security'], example:'Output pratico: dashboard requirements + telemetry checklist.' },
      { id:'sdk', tag:'Extend', title:'SDK, CLI & Custom Agents', summary:'Rendere agenti custom compatibili con il control plane.', bullets:['Agent 365 SDK','Agent 365 CLI','Identity onboarding','CI/CD e automation'], example:'Output pratico: custom-agent onboarding checklist e pipeline di esempio.' },
      { id:'operating', tag:'Operate', title:'Operating Model', summary:'Trasformare le capability tecniche in un servizio governabile.', bullets:['RACI e ownership','Runbook e incident response','KPI e review periodiche','Assessment, POC e roadmap'], example:'Output pratico: operating model + roadmap 30/60/90 giorni.' }
    ],
    examplesTitle: 'Esempi pronti da usare',
    examplesIntro: 'La repository include asset riutilizzabili per assessment, troubleshooting e delivery.',
    examples: [
      ['KQL · inventory', 'Query di base su AgentsInfo per inventario e segmentazione.'],
      ['KQL · governance gaps', 'Individuazione di agenti senza owner o con metadata incompleti.'],
      ['KQL · MCP/tools', 'Ricerca di agenti associati a tool e server MCP.'],
      ['Tenant readiness', 'Checklist prerequisiti, ruoli, licensing e accessi.'],
      ['Identity Design Sheet', 'Template per definire identità, trust, token e resource access.'],
      ['Tool Risk Register', 'Template per classificare strumenti e mitigazioni.'],
      ['Data Interaction Matrix', 'Mappa di dati, azioni, policy Purview ed evidenze.'],
      ['Customer Discovery', 'Domande strutturate per assessment e POC.'],
      ['Agent Incident Runbook', 'Checklist operativa per triage, containment, investigation, recovery e chiusura.']
    ],
    trainingTitle: 'Academy: 3 settimane, 45–55 ore',
    trainingIntro: 'Il percorso non misura quante pagine sono state lette: misura evidenze prodotte. Ogni modulo termina con un artefatto o una prova pratica.',
    weeks: [
      { title:'Settimana 1 · Foundations & Governance', days:['Mental model e architettura','Licensing, ruoli e readiness','Registry, Map e inventory','Entra Agent ID','Lifecycle e governance'] },
      { title:'Settimana 2 · Security & Data', days:['Conditional Access','Defender & AgentsInfo','Threat scenarios e incident response','Purview & DLP','Tool / MCP governance'] },
      { title:'Settimana 3 · Extend & Deliver', days:['Observability','SDK & CLI','Connected platforms','Operating model & discovery','Capstone, demo e POC'] }
    ],
    labsTitle: 'Laboratori',
    labsIntro: 'I lab trasformano la conoscenza in competenza operativa. Ogni lab produce un output verificabile.',
    labs: [
      ['LAB 01', 'Tenant readiness & baseline', 'Ruoli, licenze, accessi, inventario iniziale e gap list.'],
      ['LAB 02', 'Registry & ownership', 'Analisi inventory, owner mancanti, classification e lifecycle.'],
      ['LAB 03', 'Agent ID & Conditional Access', 'Blueprint, identity model, sponsor e access policy.'],
      ['LAB 04', 'Defender hunting', 'Query AgentsInfo, hunting hypothesis e findings.'],
      ['LAB 05', 'Purview data controls', 'Audit, data interaction matrix e DLP design.'],
      ['LAB 06', 'MCP & tool governance', 'Catalogo tool, risk rating e policy decision.'],
      ['LAB 07', 'Custom agent onboarding', 'SDK/CLI, identity, observability e checklist di integrazione.'],
      ['LAB 08', 'Local golden-agent runner', 'Foundry Local, inference reale, multi-turn e report JSON verificabile.'],
      ['LAB 09', 'Operational hardening', 'Readiness, access key opzionale, rate limit e limiti di sessione/input.'],
      ['LAB 10', 'Tool governance runtime', 'Registry, risk tier, block/unblock, approval one-shot e audit allow/deny.'],
      ['LAB 11', 'Run evidence & correlation', 'Run ID, trace ID, evidence metadata-only e correlation con il tool audit.'],
      ['LAB 12', 'Operations dashboard', 'Evidence export, KPI, filtri, run drill-down e tool decision correlation.'],
      ['LAB 13', 'Governed MCP server', 'Stdio reale, tool discovery, risk metadata, block policy, approval operatore e audit.'],
      ['LAB 14', 'Reliability & incident operations', 'Soglie, finding, error taxonomy, incident snapshot e recovery gate.'],
      ['CAPSTONE', 'Customer-ready POC', 'Assessment, architecture, controls, demo, runbook e roadmap.']
    ],
    developerTitle: 'Developer path: dall’agente esistente ad Agent 365',
    developerIntro: 'Un percorso concreto per registrare, instrumentare, validare e governare un agente custom senza riscriverne runtime o modello.',
    developerSteps: [
      ['01', 'Setup & Register', 'Installa Agent 365 Skills, verifica i prerequisiti, crea blueprint e agent identity.', 'Tutorial onboarding', 'docs/tutorials/01-onboard-existing-agent.md'],
      ['02', 'Instrument', 'Aggiungi Microsoft OpenTelemetry Distro e valida prima localmente, poi verso Agent 365.', 'Observability tutorial', 'docs/tutorials/02-observability.md'],
      ['03', 'Protect & Govern', 'Collega Defender hunting, Purview DLP e tool/MCP governance al flusso operativo.', 'Security tutorials', 'docs/tutorials/README.md'],
      ['04', 'Validate & Operate', 'Controlla root span, licensing, auth, evidence e troubleshooting prima del go-live.', 'Reference samples', 'examples/reference-agent/README.md'],
      ['05', 'Run the Golden Agent', 'Esegui il sample .NET reale con API, sessioni, tool, Docker e observability S2S opzionale.', 'Golden Agent .NET', 'samples/dotnet-golden-agent/README.md'],
      ['06', 'Local LLM', 'Avvia Foundry Local, scarica un modello adatto all’hardware e usa l’endpoint OpenAI-compatible in locale.', 'Foundry Local', 'docs/tutorials/07-foundry-local.md'],
      ['07', 'Prove it', 'Esegui inference diretta, due turni Agent Framework e salva l’evidenza del lab.', 'Local lab runner', 'docs/tutorials/08-local-lab-runner.md'],
      ['08', 'Harden the POC', 'Aggiungi readiness, access key opzionale, rate limit e limiti di sessione/input.', 'Hardening tutorial', 'docs/tutorials/09-golden-agent-hardening.md'],
      ['09', 'Govern tools', 'Classifica i tool, blocca capability, richiedi approval e conserva evidence di allow/deny.', 'Tool governance', 'docs/tutorials/10-tool-governance-runtime.md'],
      ['10', 'Correlate evidence', 'Collega run ID, trace ID, conversation e decisioni tool senza salvare prompt o risposte.', 'Evidence & observability', 'docs/tutorials/11-run-evidence-observability.md'],
      ['11', 'Operate', 'Esporta evidence e analizza KPI, latency, failure e tool decision nella dashboard del sito.', 'Operations dashboard', 'docs/tutorials/12-operations-dashboard.md'],
      ['12', 'Externalize tools', 'Esegui un MCP server .NET reale con stdio, policy, audit e approval separata dal model runtime.', 'Governed MCP server', 'docs/tutorials/13-governed-mcp-server.md'],
      ['13', 'Respond', 'Applica soglie esplicabili, identifica run sospetti e usa uno snapshot incident-ready con recovery gate.', 'Reliability & incidents', 'docs/tutorials/14-reliability-incident-operations.md']
    ],
    sourcesTitle: 'Source of truth',
    sourcesIntro: 'La knowledge base rimanda sempre alla documentazione Microsoft ufficiale. Le feature in Preview vanno ricontrollate prima di una decisione di produzione.',
    footer: 'Independent technical knowledge hub · Microsoft product names belong to their respective owners.'
  },
  en: {
    eyebrow: 'KNOWLEDGE HUB · UPDATED OCTOBER 2026',
    heroTitle: 'Microsoft Agent 365, from theory to delivery.',
    heroBody: 'A bilingual technical foundation to understand, design, govern and secure AI agents with Microsoft 365, Entra, Defender and Purview. Includes examples, checklists, labs and a complete skilling path.',
    primaryCta: 'Explore architecture',
    secondaryCta: 'Training plan',
    search: 'Search a topic…',
    status: 'Product status',
    ga: 'Commercial GA since May 1, 2026',
    principle: 'Guiding principle',
    principleText: 'Agent 365 is a cross-cutting control plane: it does not “live inside Defender”. Microsoft 365 admin center, Entra, Defender and Purview cooperate across distinct planes.',
    nav: {
      overview: 'Start', journey: 'Journey', architecture: 'Architecture', domains: 'Domains', examples: 'Examples', training: 'Academy', labs: 'Labs', developer: 'Developer', operations: 'Operations', knowledge: 'Knowledge', sources: 'Sources'
    },
    overviewTitle: 'The mental model',
    overviewIntro: 'To work effectively with Agent 365, separate the control plane from agent runtimes. The value is bringing inventory, identity, access, data protection, threat protection and observability into one operating model.',
    pillars: [
      ['Observe', 'Inventory, Agent Registry, Agent Map, usage, activity and risk signals.'],
      ['Govern', 'Ownership, lifecycle, approval, policy, tool governance and accountability.'],
      ['Secure', 'Identity, least privilege, Conditional Access, posture, threat detection and data protection.'],
      ['Extend', 'SDK, CLI, connected platforms, custom agents, MCP and integrations.'],
      ['Operate', 'Runbooks, incident response, KPIs, RACI, assessments, POCs and continuous improvement.']
    ],
    architectureTitle: 'Logical architecture',
    architectureIntro: 'Agent 365 connects existing Microsoft capabilities with agent-specific controls. Each plane has a clear responsibility.',
    layers: [
      { n:'01', title:'Management plane', product:'Microsoft 365 admin center', text:'Agent overview, Agent Registry, requests, owners, administration and tenant-wide visibility.' },
      { n:'02', title:'Identity plane', product:'Microsoft Entra Agent ID', text:'Agent identity blueprints, agent identities, sponsors, authentication, authorization, Conditional Access and governance.' },
      { n:'03', title:'Security plane', product:'Microsoft Defender', text:'Posture, Advanced Hunting, AgentsInfo, threat detection, investigation and runtime protection.' },
      { n:'04', title:'Data plane', product:'Microsoft Purview', text:'Audit, DLP, sensitivity labels, eDiscovery, data security, compliance and agent-data interaction controls.' },
      { n:'05', title:'Tool & integration plane', product:'Agent 365 SDK / CLI / MCP', text:'Onboarding, tool registry, connected platforms, OpenTelemetry and Microsoft or third-party agents.' }
    ],
    domainsTitle: 'Capability domains',
    domainsIntro: 'These are the areas a technical resource must be able to explain, configure and verify.',
    domains: [
      { id:'registry', tag:'Observe', title:'Agent Registry & Inventory', summary:'The source of truth for agents in the tenant.', bullets:['Tenant-wide inventory and discovery','Ownership, status and metadata','Agent Map and relationships','Connected platforms and shadow agents'], example:'Practical output: inventory baseline with owner, platform, identity, tools, data and risk.' },
      { id:'identity', tag:'Secure', title:'Microsoft Entra Agent ID', summary:'Purpose-built identities for interactive and autonomous agents.', bullets:['Agent identity blueprint','Agent identities and sponsors','OAuth, tokens and authorization','Conditional Access and Identity Protection'], example:'Practical output: Identity Design Sheet covering trust boundaries, credential model, resource access and least privilege.' },
      { id:'defender', tag:'Secure', title:'Microsoft Defender', summary:'Posture, hunting, detection and response for agents.', bullets:['AgentsInfo in Advanced Hunting','Risk and posture','Threat detection and incident workflow','Runtime protection and investigation'], example:'Practical output: KQL query pack plus an agent incident playbook.' },
      { id:'purview', tag:'Govern', title:'Microsoft Purview', summary:'Data protection, audit and compliance for agent interactions.', bullets:['Audit and traceability','DLP and sensitivity labels','DSPM / data security','eDiscovery and compliance'], example:'Practical output: Data Interaction Matrix agent → data → action → policy → evidence.' },
      { id:'tools', tag:'Govern', title:'Tools & MCP Governance', summary:'Control which tools an agent can discover and use.', bullets:['Tool Registry and catalog','MCP servers and connectors','Tool risk tiers','Approval, least privilege and monitoring'], example:'Practical output: Tool Risk Register with owner, scope, permissions, impact and mitigations.' },
      { id:'observability', tag:'Observe', title:'Observability', summary:'Understand agent activity, performance and behavior.', bullets:['Activity and usage signals','OpenTelemetry','Logs and retention','Correlation with identity and security'], example:'Practical output: dashboard requirements plus telemetry checklist.' },
      { id:'sdk', tag:'Extend', title:'SDK, CLI & Custom Agents', summary:'Make custom agents compatible with the control plane.', bullets:['Agent 365 SDK','Agent 365 CLI','Identity onboarding','CI/CD and automation'], example:'Practical output: custom-agent onboarding checklist and sample pipeline.' },
      { id:'operating', tag:'Operate', title:'Operating Model', summary:'Turn technical capabilities into a governable service.', bullets:['RACI and ownership','Runbooks and incident response','KPIs and periodic reviews','Assessment, POC and roadmap'], example:'Practical output: operating model plus 30/60/90-day roadmap.' }
    ],
    examplesTitle: 'Ready-to-use examples',
    examplesIntro: 'The repository includes reusable assets for assessments, troubleshooting and delivery.',
    examples: [
      ['KQL · inventory', 'Baseline AgentsInfo queries for inventory and segmentation.'],
      ['KQL · governance gaps', 'Find agents with missing owners or incomplete metadata.'],
      ['KQL · MCP/tools', 'Identify agents related to tools and MCP servers.'],
      ['Tenant readiness', 'Prerequisite, role, licensing and access checklist.'],
      ['Identity Design Sheet', 'Template for identity, trust, token and resource access design.'],
      ['Tool Risk Register', 'Template to classify tools and mitigations.'],
      ['Data Interaction Matrix', 'Map data, actions, Purview controls and evidence.'],
      ['Customer Discovery', 'Structured questions for assessments and POCs.'],
      ['Agent Incident Runbook', 'Operational checklist for triage, containment, investigation, recovery and closure.']
    ],
    trainingTitle: 'Academy: 3 weeks, 45–55 hours',
    trainingIntro: 'The path does not measure pages read; it measures evidence produced. Every module ends with an artifact or practical validation.',
    weeks: [
      { title:'Week 1 · Foundations & Governance', days:['Mental model and architecture','Licensing, roles and readiness','Registry, Map and inventory','Entra Agent ID','Lifecycle and governance'] },
      { title:'Week 2 · Security & Data', days:['Conditional Access','Defender & AgentsInfo','Threat scenarios and incident response','Purview & DLP','Tool / MCP governance'] },
      { title:'Week 3 · Extend & Deliver', days:['Observability','SDK & CLI','Connected platforms','Operating model & discovery','Capstone, demo and POC'] }
    ],
    labsTitle: 'Labs',
    labsIntro: 'Labs convert knowledge into operational capability. Every lab produces a verifiable output.',
    labs: [
      ['LAB 01', 'Tenant readiness & baseline', 'Roles, licenses, access, initial inventory and gap list.'],
      ['LAB 02', 'Registry & ownership', 'Inventory analysis, missing owners, classification and lifecycle.'],
      ['LAB 03', 'Agent ID & Conditional Access', 'Blueprint, identity model, sponsor and access policy.'],
      ['LAB 04', 'Defender hunting', 'AgentsInfo queries, hunting hypotheses and findings.'],
      ['LAB 05', 'Purview data controls', 'Audit, data interaction matrix and DLP design.'],
      ['LAB 06', 'MCP & tool governance', 'Tool catalog, risk rating and policy decision.'],
      ['LAB 07', 'Custom agent onboarding', 'SDK/CLI, identity, observability and integration checklist.'],
      ['LAB 08', 'Local golden-agent runner', 'Foundry Local, real inference, multi-turn validation and a JSON evidence report.'],
      ['LAB 09', 'Operational hardening', 'Readiness, optional access key, rate limits and input/session guardrails.'],
      ['LAB 10', 'Tool governance runtime', 'Registry, risk tiers, block/unblock, one-time approval and allow/deny audit.'],
      ['LAB 11', 'Run evidence & correlation', 'Run IDs, trace IDs, metadata-only evidence and tool-audit correlation.'],
      ['LAB 12', 'Operations dashboard', 'Evidence export, KPIs, filters, run drill-down and tool-decision correlation.'],
      ['LAB 13', 'Governed MCP server', 'Real stdio, tool discovery, risk metadata, block policy, operator approval and audit.'],
      ['CAPSTONE', 'Customer-ready POC', 'Assessment, architecture, controls, demo, runbook and roadmap.']
    ],
    developerTitle: 'Developer path: from an existing agent to Agent 365',
    developerIntro: 'A practical path to register, instrument, validate and govern a custom agent without rebuilding its runtime or model.',
    developerSteps: [
      ['01', 'Setup & Register', 'Install Agent 365 Skills, validate prerequisites, create the blueprint and agent identity.', 'Onboarding tutorial', 'docs/en/tutorials/01-onboard-existing-agent.md'],
      ['02', 'Instrument', 'Add Microsoft OpenTelemetry Distro and validate locally before Agent 365 export.', 'Observability tutorial', 'docs/en/tutorials/02-observability.md'],
      ['03', 'Protect & Govern', 'Connect Defender hunting, Purview DLP and tool/MCP governance to the operating flow.', 'Security tutorials', 'docs/en/tutorials/README.md'],
      ['04', 'Validate & Operate', 'Check root spans, licensing, auth, evidence and troubleshooting before go-live.', 'Reference samples', 'examples/reference-agent/README.md'],
      ['05', 'Run the Golden Agent', 'Run the real .NET sample with API hosting, sessions, tools, Docker and optional S2S observability.', 'Golden Agent .NET', 'samples/dotnet-golden-agent/README.md'],
      ['06', 'Local LLM', 'Start Foundry Local, download a hardware-appropriate model and use the local OpenAI-compatible endpoint.', 'Foundry Local', 'docs/en/tutorials/07-foundry-local.md'],
      ['07', 'Prove it', 'Run direct inference, two Agent Framework turns and retain lab evidence.', 'Local lab runner', 'docs/en/tutorials/08-local-lab-runner.md'],
      ['08', 'Harden the POC', 'Add readiness, optional access-key protection, rate limits and input/session guardrails.', 'Hardening tutorial', 'docs/en/tutorials/09-golden-agent-hardening.md'],
      ['09', 'Govern tools', 'Classify tools, revoke capabilities, require approval and retain allow/deny evidence.', 'Tool governance', 'docs/en/tutorials/10-tool-governance-runtime.md'],
      ['10', 'Correlate evidence', 'Link run IDs, trace IDs, conversations and tool decisions without storing prompt/response content.', 'Evidence & observability', 'docs/en/tutorials/11-run-evidence-observability.md'],
      ['11', 'Operate', 'Export evidence and inspect KPIs, latency, failures and tool decisions in the site dashboard.', 'Operations dashboard', 'docs/en/tutorials/12-operations-dashboard.md'],
      ['12', 'Externalize tools', 'Run a real .NET MCP stdio server with policy, audit and operator approval outside the model runtime.', 'Governed MCP server', 'docs/en/tutorials/13-governed-mcp-server.md']
    ],
    sourcesTitle: 'Source of truth',
    sourcesIntro: 'The knowledge base always points back to official Microsoft documentation. Preview features should be revalidated before production decisions.',
    footer: 'Independent technical knowledge hub · Microsoft product names belong to their respective owners.'
  }
}
