import { Link } from 'react-router-dom'
import { copy } from '../content.js'
import { knowledgeRoute } from '../routing.js'
import PageIntro from '../PageIntro.jsx'

const resourcePaths = [
  'examples/kql/01-agent-inventory.kql',
  'examples/kql/02-governance-gaps.kql',
  'examples/kql/03-agent-tools-mcp.kql',
  'examples/checklists/tenant-readiness.md',
  'examples/templates/identity-design-sheet.md',
  'examples/templates/tool-risk-register.csv',
  'examples/templates/data-interaction-matrix.csv',
  'examples/checklists/customer-discovery.md',
  'examples/checklists/agent-incident-runbook.md'
]

export default function AssetsPage({ lang }) {
  const t = copy[lang]
  return (
    <div className="route-page">
      <section className="section">
        <PageIntro kicker="04 · REUSABLE ASSETS" title={t.examplesTitle} text={t.examplesIntro} />
        <div className="asset-list">
          {t.examples.map(([title, text], index) => (
            <Link className="asset asset-link" to={knowledgeRoute(resourcePaths[index])} key={title}>
              <span>{String(index + 1).padStart(2, '0')}</span>
              <div><h3>{title}</h3><p>{text}</p></div>
              <b>→</b>
            </Link>
          ))}
        </div>
      </section>
    </div>
  )
}
