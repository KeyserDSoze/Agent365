# KQL Operations Pack

Questo folder non è una raccolta di query indipendenti. È un piccolo **percorso di hunting operativo** da usare quando devi capire cosa c'è nel tenant, dove sono i gap e quali agenti espongono segnali relativi a tool o MCP.

This folder is not a collection of unrelated queries. It is a small **operational hunting flow** to understand what exists in the tenant, where governance gaps are and which agents expose tool/MCP-related signals.

## Percorso consigliato / Recommended sequence

1. **[01 · Agent inventory](01-agent-inventory.kql)**  
   Costruisce la baseline iniziale degli agenti disponibili in `AgentsInfo`.

2. **[02 · Governance gaps](02-governance-gaps.kql)**  
   Parte dalla baseline e cerca record senza owner o con metadata essenziali incompleti.

3. **[03 · Tools & MCP](03-agent-tools-mcp.kql)**  
   Cerca segnali di tool, connector e MCP nel metadata raw disponibile.

## Prima di eseguire / Before running

- apri Microsoft Defender Advanced Hunting;
- verifica che `AgentsInfo` sia disponibile nel tenant;
- usa l'IntelliSense/schema explorer per controllare i campi correnti;
- non assumere che tutti i campi siano valorizzati su tutte le piattaforme;
- considera queste query **starter patterns**, non detection rule pronte per produzione.

## Cosa devi produrre

Alla fine del pack dovresti avere:

- inventory baseline;
- lista di ownership/metadata gap;
- lista di agenti da approfondire per tool/MCP;
- remediation backlog;
- ipotesi di hunting successive.

## Guida completa

La spiegazione completa, con obiettivo, output atteso, adattamenti e collegamenti ai controlli Agent 365 è disponibile qui:

- [Guida italiana — KQL per Agent 365](../../docs/17-kql-agent-operations.md)
- [English guide — KQL for Agent 365](../../docs/en/17-kql-agent-operations.md)

## Collegamenti

- [Defender](../../docs/05-defender.md)
- [Tools & MCP](../../docs/07-tools-mcp.md)
- [Defender hunting tutorial](../../docs/tutorials/03-defender-hunting.md)
- [Tool governance runtime](../../docs/tutorials/10-tool-governance-runtime.md)
