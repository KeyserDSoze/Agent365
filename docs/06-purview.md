# 06 — Microsoft Purview

## Obiettivo
Proteggere i dati usati dagli agenti e rendere le interazioni auditabili.

## Domande
Quali dati legge/scrive? Può trasformarli o inviarli a tool esterni? Quali classificazioni? Quali policy DLP? Quale evidence resta disponibile?

## Capability
- Audit;
- DLP;
- sensitivity/classification;
- data security posture;
- eDiscovery;
- compliance.

## Data Interaction Matrix
Per ogni agente mappare data source, classificazione, operation, destination, policy, expected evidence, owner, risk e mitigation.

Template: `examples/templates/data-interaction-matrix.csv`.

## Anti-pattern
“L'agente vede solo ciò che vede l'utente” non basta: agenti autonomi, tool, over-privilege e aggregazione possono creare nuove trust boundary e nuovi rischi.

## Fonte
https://learn.microsoft.com/en-us/security/security-for-ai/agent-365-security
