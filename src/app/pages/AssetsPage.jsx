import { Link } from 'react-router-dom'
import { knowledgeRoute } from '../routing.js'
import PageIntro from '../PageIntro.jsx'
import JourneyNext from '../JourneyNext.jsx'

function AssetLink({ path, title, text, type, lang }) {
  return (
    <Link className="toolkit-item" to={knowledgeRoute(path)}>
      <span>{type}</span>
      <div><strong>{title}</strong><p>{text}</p></div>
      <b>→</b>
    </Link>
  )
}

export default function AssetsPage({ lang }) {
  const it = lang === 'it'

  const packs = [
    {
      id: 'assessment',
      stage: '02 · ' + (it ? 'PREPARARE' : 'PREPARE'),
      title: it ? 'Assessment & readiness pack' : 'Assessment & readiness pack',
      why: it
        ? 'Serve prima di qualunque POC: stabilisce se tenant, ruoli, identità e ownership sono pronti e rende espliciti i gap.'
        : 'Use it before any POC: it establishes whether tenant, roles, identity and ownership are ready and makes gaps explicit.',
      start: it ? 'Parti dalla Tenant Readiness, poi completa discovery e identity design.' : 'Start with Tenant Readiness, then complete discovery and identity design.',
      items: [
        ['examples/checklists/tenant-readiness.md', 'Tenant readiness', it ? 'Prerequisiti, licenze, ruoli, accessi e governance minima.' : 'Prerequisites, licenses, roles, access and minimum governance.', 'CHECKLIST'],
        ['examples/checklists/customer-discovery.md', 'Customer discovery', it ? 'Domande per trasformare una prima call in un assessment strutturato.' : 'Questions that turn an initial call into a structured assessment.', 'CHECKLIST'],
        ['examples/templates/identity-design-sheet.md', 'Identity Design Sheet', it ? 'Disegna sponsor, trust boundary, token flow, least privilege e revocation.' : 'Design sponsor, trust boundary, token flow, least privilege and revocation.', 'TEMPLATE']
      ]
    },
    {
      id: 'hunting',
      stage: '04 · ' + (it ? 'GOVERNARE' : 'GOVERN'),
      title: 'KQL Operations Pack',
      why: it
        ? 'Non sono tre query scollegate: sono una sequenza di hunting da usare per costruire inventory, trovare gap e capire l’esposizione a tool/MCP.'
        : 'These are not three unrelated queries: they form a hunting sequence to build inventory, find gaps and understand tool/MCP exposure.',
      start: it ? 'Apri prima la guida del pack: spiega schema, prerequisiti, output e come leggere ogni query.' : 'Open the pack guide first: it explains schema, prerequisites, outputs and how to use each query.',
      items: [
        ['examples/kql/README.md', it ? 'Guida al KQL Operations Pack' : 'KQL Operations Pack guide', it ? 'Perché esiste, quando usarlo e come adattarlo al tenant.' : 'Why it exists, when to use it and how to adapt it to the tenant.', 'GUIDE'],
        ['examples/kql/01-agent-inventory.kql', '01 · Agent inventory', it ? 'Baseline AgentsInfo e segmentazione iniziale.' : 'AgentsInfo baseline and initial segmentation.', 'KQL'],
        ['examples/kql/02-governance-gaps.kql', '02 · Governance gaps', it ? 'Owner e metadata mancanti da trasformare in remediation backlog.' : 'Missing owners and metadata to turn into a remediation backlog.', 'KQL'],
        ['examples/kql/03-agent-tools-mcp.kql', '03 · Tools & MCP', it ? 'Segnali su tool, connector e server MCP nel metadata disponibile.' : 'Signals for tools, connectors and MCP servers in available metadata.', 'KQL']
      ]
    },
    {
      id: 'governance',
      stage: '04 · ' + (it ? 'GOVERNARE' : 'GOVERN'),
      title: it ? 'Governance design pack' : 'Governance design pack',
      why: it
        ? 'Serve a trasformare finding tecnici in decisioni: quali tool autorizzare, quali dati possono essere toccati e quali controlli applicare.'
        : 'Use it to turn technical findings into decisions: which tools to allow, which data can be accessed and which controls apply.',
      start: it ? 'Usalo dopo inventory e identity baseline.' : 'Use it after inventory and identity baseline.',
      items: [
        ['examples/templates/tool-risk-register.csv', 'Tool Risk Register', it ? 'Risk tier, owner, scope, impatto e mitigazioni per ogni tool.' : 'Risk tier, owner, scope, impact and mitigations for each tool.', 'TEMPLATE'],
        ['examples/templates/data-interaction-matrix.csv', 'Data Interaction Matrix', it ? 'Agente → dato → azione → policy → evidence.' : 'Agent → data → action → policy → evidence.', 'TEMPLATE']
      ]
    },
    {
      id: 'operations',
      stage: '05 · ' + (it ? 'OPERARE' : 'OPERATE'),
      title: it ? 'Operations & incident pack' : 'Operations & incident pack',
      why: it
        ? 'Serve quando il POC deve diventare osservabile e gestibile: evidence, reliability, triage e recovery.'
        : 'Use it when the POC must become observable and operable: evidence, reliability, triage and recovery.',
      start: it ? 'Parti dalla dashboard operations e usa il runbook in caso di finding critico.' : 'Start from the operations dashboard and use the runbook for critical findings.',
      items: [
        ['examples/evidence/operations-sample.json', 'Operations evidence sample', it ? 'Bundle metadata-only usato dalla dashboard per KPI e drill-down.' : 'Metadata-only bundle used by the dashboard for KPIs and drill-down.', 'JSON'],
        ['examples/checklists/agent-incident-runbook.md', 'Agent Incident Runbook', it ? 'Triage, containment, investigation, recovery e chiusura.' : 'Triage, containment, investigation, recovery and closure.', 'RUNBOOK']
      ]
    }
  ]

  return (
    <div className="route-page">
      <section className="section toolkit-page">
        <PageIntro
          kicker={it ? '02 / 04 / 05 · TOOLKIT' : '02 / 04 / 05 · TOOLKIT'}
          title={it ? 'Gli asset hanno senso solo dentro un lavoro.' : 'Assets only make sense inside a job.'}
          text={it
            ? 'Qui non trovi più una lista piatta di file. Ogni pack spiega quando usarlo, che problema risolve e in quale tappa del percorso entra.'
            : 'This is no longer a flat list of files. Every pack explains when to use it, what problem it solves and where it belongs in the journey.'}
        />

        <div className="toolkit-packs">
          {packs.map(pack => (
            <article className="toolkit-pack" key={pack.id}>
              <div className="toolkit-pack-intro">
                <span>{pack.stage}</span>
                <h2>{pack.title}</h2>
                <p>{pack.why}</p>
                <strong>{pack.start}</strong>
              </div>
              <div className="toolkit-items">
                {pack.items.map(([path, title, text, type]) => (
                  <AssetLink path={path} title={title} text={text} type={type} lang={lang} key={path} />
                ))}
              </div>
            </article>
          ))}
        </div>

        <JourneyNext
          lang={lang}
          stageId="prepare"
          title={it
            ? 'Dopo la baseline, scegli se costruire un POC oppure entrare direttamente sui controlli di governance.'
            : 'After the baseline, choose whether to build a POC or move directly into governance controls.'}
        >
          <div className="journey-inline-links">
            <Link to="/developer">{it ? 'Vai al developer path' : 'Go to developer path'} →</Link>
            <Link to="/capabilities">{it ? 'Vai ai domini di governance' : 'Go to governance domains'} →</Link>
          </div>
        </JourneyNext>
      </section>
    </div>
  )
}
