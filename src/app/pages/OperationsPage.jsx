import OperationsDashboard from '../OperationsDashboard.jsx'
import PageIntro from '../PageIntro.jsx'
import JourneyNext from '../JourneyNext.jsx'

export default function OperationsPage({ lang }) {
  return (
    <div className="route-page">
      <section className="section operations">
        <PageIntro
          kicker="08 · OPERATIONS"
          title={lang === 'it' ? 'Dal run alla evidence, con reliability e incident response.' : 'From runs to evidence, reliability and incident response.'}
          text={lang === 'it'
            ? 'KPI, latency, failure, correlation, decisioni tool e finding operativi in una pagina dedicata.'
            : 'KPIs, latency, failures, correlation, tool decisions and operational findings in a dedicated page.'}
        />
        <OperationsDashboard lang={lang} />
        <JourneyNext
          lang={lang}
          stageId="operate"
          title={lang === 'it'
            ? 'La dashboard è la vista operativa: per chiudere il cerchio usa runbook, recovery gate e customer delivery pack.'
            : 'The dashboard is the operational view: close the loop with runbooks, recovery gates and the customer delivery pack.'}
        />
      </section>
    </div>
  )
}
