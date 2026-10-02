# Tutorial 03 — Defender Advanced Hunting for Agent 365

## Goal

Creare un hunting pack utile a inventory, governance e security review.

> Verificare sempre lo schema live di `AgentsInfo` nel tenant. Le colonne possono evolvere.

## Query 1 — Last seen by agent

```kusto
AgentsInfo
| summarize LastSeen=max(Timestamp) by AgentId, AgentName, Platform, AgentType
| order by LastSeen desc
```

## Query 2 — Platform distribution

```kusto
AgentsInfo
| summarize Agents=dcount(AgentId) by Platform
| order by Agents desc
```

## Query 3 — Potential governance gaps

Adattare i nomi colonna dopo aver verificato lo schema:

```kusto
AgentsInfo
| where isempty(Owner) or isempty(AgentName)
| project Timestamp, AgentId, AgentName, Platform, Owner, Status
```

## Query 4 — MCP/tool hints in raw metadata

```kusto
AgentsInfo
| where tostring(RawAgentInfo) has_any ("MCP", "tool", "connector")
| project Timestamp, AgentId, AgentName, Platform, RawAgentInfo
```

## Query 5 — Daily discovery trend

```kusto
AgentsInfo
| summarize Agents=dcount(AgentId) by bin(Timestamp, 1d)
| order by Timestamp asc
```

## Hunting workflow

1. Inspect schema.
2. Build baseline.
3. Segment per platform.
4. Identify owner/metadata gaps.
5. Search tool/MCP signals.
6. Compare with approved inventory.
7. Open findings.
8. Assign owner and due date.

## Evidence

Per ogni finding salvare:
- query;
- timestamp;
- agent ID;
- screenshot/export;
- interpretation;
- false-positive check;
- remediation owner.

## Starter files

Vedi `examples/kql/advanced/`.
