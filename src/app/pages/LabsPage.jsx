import { Link } from 'react-router-dom'
import { copy } from '../content.js'
import { knowledgeRoute } from '../routing.js'
import PageIntro from '../PageIntro.jsx'

export default function LabsPage({ lang }) {
  const t = copy[lang]
  const it = lang === 'it'

  const links = {
    'LAB 01': 'examples/checklists/tenant-readiness.md',
    'LAB 02': it ? 'docs/03-registry-governance.md' : 'docs/en/03-registry-governance.md',
    'LAB 03': it ? 'docs/04-entra-agent-id.md' : 'docs/en/04-entra-agent-id.md',
    'LAB 04': it ? 'docs/tutorials/03-defender-hunting.md' : 'docs/en/tutorials/03-defender-hunting.md',
    'LAB 05': it ? 'docs/tutorials/04-purview-dlp.md' : 'docs/en/tutorials/04-purview-dlp.md',
    'LAB 06': it ? 'docs/07-tools-mcp.md' : 'docs/en/07-tools-mcp.md',
    'LAB 07': it ? 'docs/tutorials/01-onboard-existing-agent.md' : 'docs/en/tutorials/01-onboard-existing-agent.md',
    'LAB 08': it ? 'docs/tutorials/08-local-lab-runner.md' : 'docs/en/tutorials/08-local-lab-runner.md',
    'LAB 09': it ? 'docs/tutorials/09-golden-agent-hardening.md' : 'docs/en/tutorials/09-golden-agent-hardening.md',
    'LAB 10': it ? 'docs/tutorials/10-tool-governance-runtime.md' : 'docs/en/tutorials/10-tool-governance-runtime.md',
    'LAB 11': it ? 'docs/tutorials/11-run-evidence-observability.md' : 'docs/en/tutorials/11-run-evidence-observability.md',
    'LAB 12': it ? 'docs/tutorials/12-operations-dashboard.md' : 'docs/en/tutorials/12-operations-dashboard.md',
    'LAB 13': it ? 'docs/tutorials/13-governed-mcp-server.md' : 'docs/en/tutorials/13-governed-mcp-server.md',
    'LAB 14': it ? 'docs/tutorials/14-reliability-incident-operations.md' : 'docs/en/tutorials/14-reliability-incident-operations.md',
    'CAPSTONE': it ? 'docs/13-customer-delivery.md' : 'docs/en/13-customer-delivery.md'
  }

  const stages = {
    'LAB 01': 'PREPARE', 'LAB 02': 'PREPARE', 'LAB 03': 'PREPARE',
    'LAB 04': 'GOVERN', 'LAB 05': 'GOVERN', 'LAB 06': 'GOVERN',
    'LAB 07': 'BUILD', 'LAB 08': 'BUILD', 'LAB 09': 'BUILD', 'LAB 10': 'GOVERN',
    'LAB 11': 'OPERATE', 'LAB 12': 'OPERATE', 'LAB 13': 'BUILD', 'LAB 14': 'OPERATE',
    'CAPSTONE': 'DELIVER'
  }

  return (
    <div className="route-page">
      <section className="section">
        <PageIntro
          kicker="HANDS-ON"
          title={t.labsTitle}
          text={it
            ? 'Ogni lab è ora un punto di ingresso reale: cliccandolo apri il tutorial, il capitolo o l’asset necessario per eseguirlo e produrre l’evidenza prevista.'
            : 'Every lab is now a real entry point: open it to reach the tutorial, chapter or asset required to execute it and produce the expected evidence.'}
        />
        <div className="lab-grid">
          {t.labs.map(([code, title, text]) => (
            <Link className="lab lab-link" to={knowledgeRoute(links[code])} key={code}>
              <div className="lab-meta"><span>{code}</span><small>{stages[code]}</small></div>
              <h3>{title}</h3>
              <p>{text}</p>
              <strong>{it ? 'Apri materiale operativo' : 'Open operational material'} →</strong>
            </Link>
          ))}
        </div>

        <div className="context-links">
          <Link to="/journey">{it ? 'Non sai da quale lab partire? Torna al percorso' : 'Not sure which lab to start with? Return to the journey'} →</Link>
          <Link to="/developer">{it ? 'Stai costruendo? Segui il developer path' : 'Building something? Follow the developer path'} →</Link>
        </div>
      </section>
    </div>
  )
}
