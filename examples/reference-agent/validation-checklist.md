# Reference integration validation checklist

## Registration
- [ ] Agent 365 setup completed
- [ ] Blueprint exists
- [ ] Agent identity exists
- [ ] Owner/sponsor documented
- [ ] No secrets committed

## Observability
- [ ] Console telemetry works locally
- [ ] Agent 365 exporter intentionally enabled/disabled
- [ ] Token resolver implemented
- [ ] Auth flow is OBO or S2S by design, not accident
- [ ] Valid invoke_agent root span exists
- [ ] Test timestamp recorded
- [ ] Agent visible in expected surfaces

## Tools
- [ ] Tool list documented
- [ ] Work IQ MCP Preview status accepted where used
- [ ] M365 Copilot licensing verified where required
- [ ] Admin consent completed
- [ ] Least privilege checked

## Data
- [ ] Input DLP requirement decided
- [ ] Data Interaction Matrix completed
- [ ] Logging of sensitive content reviewed

## Operations
- [ ] Incident owner
- [ ] Revocation path
- [ ] Troubleshooting evidence template
- [ ] POC exit criteria
