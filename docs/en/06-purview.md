# 06 — Microsoft Purview

## Goal
Protect data used by agents and make agent-data interaction auditable.

## Key questions
What data can the agent read or write? Can it transform or send data to external tools? What classifications apply? Which DLP controls exist? Which evidence remains available?

## Capabilities
Audit, DLP, sensitivity/classification, data security posture, eDiscovery and compliance.

## Data Interaction Matrix
Map data source, classification, operation, destination, control, expected evidence, owner, risk and mitigation.

Template: `../../examples/templates/data-interaction-matrix.csv`.

## Anti-pattern
“An agent only sees what the user can see” is not enough. Autonomous agents, tool boundaries, over-privileged users and aggregation can create new risks.

## Source
https://learn.microsoft.com/en-us/security/security-for-ai/agent-365-security
