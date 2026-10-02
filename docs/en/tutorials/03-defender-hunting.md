# Tutorial 03 — Defender Advanced Hunting

## Goal
Build a repeatable Agent 365 hunting pack for inventory, governance and security review.

Always inspect the live `AgentsInfo` schema first.

## Last seen
```kusto
AgentsInfo
| summarize LastSeen=max(Timestamp) by AgentId, AgentName, Platform, AgentType
| order by LastSeen desc
```

## Platform distribution
```kusto
AgentsInfo
| summarize Agents=dcount(AgentId) by Platform
| order by Agents desc
```

## Governance gap starter
```kusto
AgentsInfo
| where isempty(Owner) or isempty(AgentName)
| project Timestamp, AgentId, AgentName, Platform, Owner, Status
```

## MCP/tool hints
```kusto
AgentsInfo
| where tostring(RawAgentInfo) has_any ("MCP", "tool", "connector")
| project Timestamp, AgentId, AgentName, Platform, RawAgentInfo
```

## Hunting workflow
Inspect schema → baseline → segment → identify governance gaps → search tool/MCP signals → compare against approved inventory → assign findings.

For every finding keep the query, timestamp, agent ID, export/screenshot, interpretation, false-positive check and remediation owner.
