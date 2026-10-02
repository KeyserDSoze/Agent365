# 05 — Microsoft Defender

## Obiettivo
Visibility, posture, hunting, detection e response sugli agenti.

## Advanced Hunting
Per Agent 365 Microsoft indirizza verso **`AgentsInfo`**. Il precedente schema `AIAgentsInfo` è stato sostituito nel percorso 2026. Verificare sempre lo schema corrente del tenant prima di riusare query.

## Hunting
Cercare owner mancanti, status inattesi, piattaforme non previste, permessi elevati, tool/MCP ad alto impatto, modifiche recenti e configurazioni fuori standard.

Query in `examples/kql/`.

## Incident model
1. identity compromise;
2. instruction/prompt manipulation;
3. tool misuse;
4. data exfiltration/oversharing;
5. anomalous behavior;
6. connected-platform risk;
7. misconfiguration.

## Triage
Identificare agente/owner/runtime/identity/tool/data; ricostruire timeline; limitare accessi; preservare evidence; correggere config; validare recovery.

## Transizione 2026
Microsoft ha consolidato in Agent 365 diverse capability security di Copilot Studio/Foundry e ha spostato l'inventory verso `AgentsInfo`, con cambiamenti operativi dal 1 luglio 2026.

## Fonti
- https://learn.microsoft.com/en-us/security/security-for-ai/agent-365-security
- https://learn.microsoft.com/en-us/defender-xdr/security-for-ai/transition-agent-security-to-agent-365
- https://learn.microsoft.com/en-us/microsoft-365/security/defender/advanced-hunting-schema-changes?view=o365-worldwide
