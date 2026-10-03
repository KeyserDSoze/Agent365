import { useEffect, useMemo, useRef, useState } from 'react'

function shortId(value, size = 10) {
  if (!value) return '—'
  return value.length <= size ? value : `${value.slice(0, size)}…`
}

function formatMs(value) {
  const n = Number(value || 0)
  if (n >= 1000) return `${(n / 1000).toFixed(2)}s`
  return `${Math.round(n)}ms`
}

function formatTime(value, lang) {
  if (!value) return '—'
  try {
    return new Intl.DateTimeFormat(lang === 'it' ? 'it-IT' : 'en-GB', {
      hour: '2-digit',
      minute: '2-digit',
      second: '2-digit'
    }).format(new Date(value))
  } catch {
    return value
  }
}

function calculateSummary(runs) {
  if (!runs.length) {
    return {
      bufferedRuns: 0,
      successfulRuns: 0,
      failedRuns: 0,
      averageDurationMs: 0,
      toolInvocations: 0,
      toolAllowed: 0,
      toolDenied: 0
    }
  }

  return {
    bufferedRuns: runs.length,
    successfulRuns: runs.filter(x => x.status === 'completed').length,
    failedRuns: runs.filter(x => x.status === 'failed').length,
    averageDurationMs: runs.reduce((sum, x) => sum + Number(x.durationMs || 0), 0) / runs.length,
    toolInvocations: runs.reduce((sum, x) => sum + Number(x.toolInvocations || 0), 0),
    toolAllowed: runs.reduce((sum, x) => sum + Number(x.toolAllowed || 0), 0),
    toolDenied: runs.reduce((sum, x) => sum + Number(x.toolDenied || 0), 0)
  }
}

function normalizeBundle(raw) {
  const runs = Array.isArray(raw?.runs)
    ? raw.runs
    : Array.isArray(raw?.items)
      ? raw.items
      : []

  return {
    schema: raw?.schema || 'agent365-golden-agent-evidence/v1',
    generatedAt: raw?.generatedAt || new Date().toISOString(),
    capturesContent: raw?.capturesContent === true,
    runs,
    summary: raw?.summary || calculateSummary(runs),
    toolAudit: Array.isArray(raw?.toolAudit) ? raw.toolAudit : [],
    tools: Array.isArray(raw?.tools) ? raw.tools : [],
    reliability: raw?.reliability || null
  }
}

export default function OperationsDashboard({ lang }) {
  const fileRef = useRef(null)
  const [bundle, setBundle] = useState(() => normalizeBundle({}))
  const [source, setSource] = useState('sample')
  const [error, setError] = useState('')
  const [status, setStatus] = useState('all')
  const [provider, setProvider] = useState('all')
  const [selectedRunId, setSelectedRunId] = useState('')

  const sampleUrl = `${import.meta.env.BASE_URL}content/examples/evidence/operations-sample.json`

  const loadSample = async () => {
    setError('')

    try {
      const response = await fetch(sampleUrl)
      if (!response.ok) throw new Error(`HTTP ${response.status}`)
      const data = normalizeBundle(await response.json())
      setBundle(data)
      setSource('sample')
      setSelectedRunId(data.runs[0]?.runId || '')
    } catch (err) {
      setError(err.message)
    }
  }

  useEffect(() => {
    loadSample()
    // sample URL is static for the current Vite base.
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [])

  const handleFile = async event => {
    const file = event.target.files?.[0]
    if (!file) return

    setError('')

    try {
      const parsed = JSON.parse(await file.text())
      const data = normalizeBundle(parsed)

      if (!data.runs.length) {
        throw new Error(lang === 'it'
          ? 'Il file non contiene run evidence.'
          : 'The file does not contain run evidence.')
      }

      setBundle(data)
      setSource(file.name)
      setSelectedRunId(data.runs[0]?.runId || '')
      setStatus('all')
      setProvider('all')
    } catch (err) {
      setError(err.message)
    } finally {
      event.target.value = ''
    }
  }

  const providers = useMemo(() => {
    return [...new Set(bundle.runs.map(x => x.provider).filter(Boolean))].sort()
  }, [bundle.runs])

  const runs = useMemo(() => {
    return [...bundle.runs]
      .filter(run => status === 'all' || run.status === status)
      .filter(run => provider === 'all' || run.provider === provider)
      .sort((a, b) => new Date(b.timestamp) - new Date(a.timestamp))
  }, [bundle.runs, status, provider])

  const summary = useMemo(() => calculateSummary(bundle.runs), [bundle.runs])
  const selected = bundle.runs.find(x => x.runId === selectedRunId) || runs[0] || null
  const selectedAudit = selected
    ? bundle.toolAudit.filter(x => x.runId === selected.runId)
    : []

  const maxLatency = Math.max(1, ...runs.map(x => Number(x.durationMs || 0)))
  const successRate = summary.bufferedRuns
    ? Math.round((summary.successfulRuns / summary.bufferedRuns) * 100)
    : 0
  const reliability = bundle.reliability
  const errorTypes = reliability?.errorTypes
    ? Object.entries(reliability.errorTypes)
    : []

  const inspectFinding = finding => {
    const runId = finding?.runIds?.[0]
    if (runId) {
      setSelectedRunId(runId)
      setStatus('all')
      setProvider('all')
    }
  }

  return (
    <div className="ops-shell">
      <div className="ops-toolbar">
        <div>
          <span className="ops-live-dot" />
          <div>
            <strong>{lang === 'it' ? 'Operations evidence' : 'Operations evidence'}</strong>
            <small>
              {source === 'sample'
                ? (lang === 'it' ? 'Sample incluso nella repository' : 'Repository sample')
                : source}
            </small>
          </div>
        </div>

        <div className="ops-actions">
          <button onClick={() => fileRef.current?.click()}>
            {lang === 'it' ? 'Carica export JSON' : 'Load JSON export'}
          </button>
          <button onClick={loadSample}>
            {lang === 'it' ? 'Ripristina sample' : 'Reset sample'}
          </button>
          <input
            ref={fileRef}
            type="file"
            accept=".json,application/json"
            onChange={handleFile}
            hidden
          />
        </div>
      </div>

      {error && <div className="ops-error">{error}</div>}

      <div className="ops-privacy">
        <span>{bundle.capturesContent ? '!' : '✓'}</span>
        <div>
          <strong>
            {bundle.capturesContent
              ? (lang === 'it' ? 'Content capture dichiarato' : 'Content capture declared')
              : (lang === 'it' ? 'Metadata-only evidence' : 'Metadata-only evidence')}
          </strong>
          <small>
            {lang === 'it'
              ? 'La dashboard lavora in locale nel browser. Il sample non contiene prompt o risposte.'
              : 'The dashboard runs locally in the browser. The sample contains no prompts or responses.'}
          </small>
        </div>
      </div>

      {reliability && (
        <div className={`ops-reliability ${reliability.status}`}>
          <div className="ops-reliability-head">
            <div>
              <span>RELIABILITY</span>
              <strong>{reliability.status}</strong>
            </div>
            <div className="ops-score">
              <small>{lang === 'it' ? 'Score trasparente' : 'Transparent score'}</small>
              <strong>{reliability.score ?? '—'}</strong>
            </div>
          </div>

          <div className="ops-reliability-metrics">
            <div>
              <span>Failure rate</span>
              <strong>{Math.round(Number(reliability.failureRate || 0) * 100)}%</strong>
            </div>
            <div>
              <span>{lang === 'it' ? 'Latency media' : 'Avg latency'}</span>
              <strong>{formatMs(reliability.averageLatencyMs)}</strong>
            </div>
            <div>
              <span>Tool deny rate</span>
              <strong>{Math.round(Number(reliability.toolDenyRate || 0) * 100)}%</strong>
            </div>
            <div>
              <span>{lang === 'it' ? 'Failure consecutive' : 'Consecutive failures'}</span>
              <strong>{reliability.consecutiveFailures || 0}</strong>
            </div>
          </div>

          <div className="ops-findings">
            <div className="ops-findings-title">
              <span>{lang === 'it' ? 'FINDING OPERATIVI' : 'OPERATIONAL FINDINGS'}</span>
              <strong>{reliability.findings?.length || 0}</strong>
            </div>

            {(reliability.findings || []).map(finding => (
              <button
                key={finding.code}
                className={finding.severity}
                onClick={() => inspectFinding(finding)}
                disabled={!finding.runIds?.length}
              >
                <span>{finding.severity}</span>
                <div>
                  <strong>{finding.title}</strong>
                  <p>{finding.detail}</p>
                  <small>{finding.recommendedAction}</small>
                </div>
                <b>{finding.runIds?.length ? '→' : '·'}</b>
              </button>
            ))}

            {!reliability.findings?.length && (
              <p className="ops-empty">
                {lang === 'it'
                  ? 'Nessuna soglia operativa superata.'
                  : 'No operational threshold crossed.'}
              </p>
            )}
          </div>

          {!!errorTypes.length && (
            <div className="ops-errors">
              <span>{lang === 'it' ? 'ERROR TAXONOMY' : 'ERROR TAXONOMY'}</span>
              <div>
                {errorTypes.map(([name, count]) => (
                  <small key={name}><b>{count}</b> {name}</small>
                ))}
              </div>
            </div>
          )}
        </div>
      )}

      <div className="ops-kpis">
        <article>
          <span>{lang === 'it' ? 'Run' : 'Runs'}</span>
          <strong>{summary.bufferedRuns}</strong>
          <small>{successRate}% {lang === 'it' ? 'successo' : 'success'}</small>
        </article>
        <article>
          <span>{lang === 'it' ? 'Latency media' : 'Avg latency'}</span>
          <strong>{formatMs(summary.averageDurationMs)}</strong>
          <small>{lang === 'it' ? 'sui run caricati' : 'across loaded runs'}</small>
        </article>
        <article>
          <span>{lang === 'it' ? 'Failure' : 'Failures'}</span>
          <strong>{summary.failedRuns}</strong>
          <small>{lang === 'it' ? 'run non completati' : 'incomplete runs'}</small>
        </article>
        <article>
          <span>Tool deny</span>
          <strong>{summary.toolDenied}</strong>
          <small>{summary.toolAllowed} allowed / {summary.toolInvocations} total</small>
        </article>
      </div>

      <div className="ops-grid">
        <section className="ops-panel ops-runs">
          <div className="ops-panel-head">
            <div>
              <span>RUNS</span>
              <strong>{runs.length}</strong>
            </div>
            <div className="ops-filters">
              <select value={status} onChange={e => setStatus(e.target.value)}>
                <option value="all">{lang === 'it' ? 'Tutti gli stati' : 'All statuses'}</option>
                <option value="completed">completed</option>
                <option value="failed">failed</option>
              </select>
              <select value={provider} onChange={e => setProvider(e.target.value)}>
                <option value="all">{lang === 'it' ? 'Tutti i provider' : 'All providers'}</option>
                {providers.map(x => <option key={x} value={x}>{x}</option>)}
              </select>
            </div>
          </div>

          <div className="ops-run-list">
            {runs.map(run => (
              <button
                key={run.runId}
                className={selected?.runId === run.runId ? 'active' : ''}
                onClick={() => setSelectedRunId(run.runId)}
              >
                <span className={`ops-status ${run.status}`} />
                <div className="ops-run-main">
                  <strong>{shortId(run.runId, 14)}</strong>
                  <small>{run.provider} · {shortId(run.model, 28)}</small>
                </div>
                <div className="ops-latency">
                  <i style={{ width: `${Math.max(7, (Number(run.durationMs || 0) / maxLatency) * 100)}%` }} />
                  <small>{formatMs(run.durationMs)}</small>
                </div>
                <time>{formatTime(run.timestamp, lang)}</time>
              </button>
            ))}

            {!runs.length && (
              <p className="ops-empty">
                {lang === 'it' ? 'Nessun run con questi filtri.' : 'No runs match these filters.'}
              </p>
            )}
          </div>
        </section>

        <aside className="ops-panel ops-detail">
          <div className="ops-panel-head">
            <div>
              <span>{lang === 'it' ? 'RUN DETAIL' : 'RUN DETAIL'}</span>
              <strong>{selected ? shortId(selected.runId, 18) : '—'}</strong>
            </div>
          </div>

          {selected && (
            <>
              <dl className="ops-meta">
                <div><dt>Status</dt><dd className={selected.status}>{selected.status}</dd></div>
                <div><dt>Conversation</dt><dd title={selected.conversationId}>{shortId(selected.conversationId, 20)}</dd></div>
                <div><dt>Trace</dt><dd title={selected.traceId}>{shortId(selected.traceId, 20)}</dd></div>
                <div><dt>Provider</dt><dd>{selected.provider}</dd></div>
                <div><dt>Model</dt><dd title={selected.model}>{shortId(selected.model, 25)}</dd></div>
                <div><dt>Duration</dt><dd>{formatMs(selected.durationMs)}</dd></div>
                <div><dt>Input size</dt><dd>{selected.requestCharacters} chars</dd></div>
                <div><dt>Output size</dt><dd>{selected.responseCharacters} chars</dd></div>
              </dl>

              <div className="ops-tool-head">
                <span>{lang === 'it' ? 'TOOL DECISIONS' : 'TOOL DECISIONS'}</span>
                <strong>{selectedAudit.length}</strong>
              </div>

              <div className="ops-tool-list">
                {selectedAudit.map((item, i) => (
                  <article key={`${item.toolName}-${i}`} className={item.decision}>
                    <div>
                      <strong>{item.toolName}</strong>
                      <small>{item.operation} · {item.riskTier}</small>
                    </div>
                    <span>{item.decision}</span>
                    {item.reason && <p>{item.reason}</p>}
                  </article>
                ))}

                {!selectedAudit.length && (
                  <p className="ops-empty">
                    {lang === 'it' ? 'Nessuna tool invocation per questo run.' : 'No tool invocation for this run.'}
                  </p>
                )}
              </div>
            </>
          )}
        </aside>
      </div>

      <div className="ops-footer">
        <code>{bundle.schema}</code>
        <span>
          {lang === 'it' ? 'Generato' : 'Generated'} · {formatTime(bundle.generatedAt, lang)}
        </span>
      </div>
    </div>
  )
}
