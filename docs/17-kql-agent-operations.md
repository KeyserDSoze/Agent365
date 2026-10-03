# KQL per Agent 365 — inventory, governance gap e tool/MCP

## Perché esiste questo pack

Le query KQL nella repository non servono a “fare una demo di Advanced Hunting”. Servono a trasformare la visibilità tecnica in un processo operativo:

**inventory → gap → approfondimento → remediation → evidence**

Il pack è volutamente piccolo. L'obiettivo è insegnare come partire da `AgentsInfo`, verificare lo schema realmente disponibile nel tenant e costruire poi hunting query più specifiche.

## Dove si usa

Le query sono pensate per **Microsoft Defender Advanced Hunting**.

Prima di usarle:

1. verifica che `AgentsInfo` sia disponibile;
2. apri lo schema corrente nel tenant;
3. controlla nomi e valorizzazione dei campi;
4. annota la piattaforma di provenienza degli agenti;
5. tratta i pattern della repository come punto di partenza, non come detection rule production-ready.

## Il flusso completo

### 1. Costruisci la baseline

Apri [01-agent-inventory.kql](../examples/kql/01-agent-inventory.kql).

Obiettivo:

- vedere gli agenti osservabili;
- identificare `AgentId`, nome, tipo, piattaforma e stato;
- capire se il dataset è abbastanza completo per procedere.

Output consigliato:

| Campo | Uso |
|---|---|
| AgentId | Chiave di correlazione |
| AgentName | Identificazione leggibile |
| AgentType | Segmentazione |
| Platform | Provenienza/runtime |
| Status | Stato osservato |
| Timestamp | Freshness dell'evidence |

**Non passare subito alle remediation.** Prima salva una baseline e verifica se i record sono coerenti con ciò che il team si aspetta di trovare.

### 2. Trova i gap di governance

Apri [02-governance-gaps.kql](../examples/kql/02-governance-gaps.kql).

Questa query cerca un primo insieme di problemi semplici:

- owner assente;
- nome/metadata essenziali mancanti.

Il valore non è il filtro in sé: è costruire un **backlog di remediation**.

Per ogni finding aggiungi almeno:

- agent ID;
- piattaforma;
- owner atteso;
- owner effettivo;
- criticità;
- azione;
- responsabile della remediation;
- data di review.

Collega questa attività a:

- [Registry & Governance](03-registry-governance.md);
- [Entra Agent ID](04-entra-agent-id.md);
- [Tenant readiness](../examples/checklists/tenant-readiness.md).

### 3. Cerca esposizione a tool e MCP

Apri [03-agent-tools-mcp.kql](../examples/kql/03-agent-tools-mcp.kql).

Il pattern usa `RawAgentInfo` per cercare segnali testuali relativi a:

- MCP;
- tool;
- connector.

È un **discovery pattern**, non una prova definitiva che un determinato tool sia attivo o rischioso.

Quando trovi un candidato:

1. apri il record completo;
2. identifica piattaforma e agent ID;
3. verifica il catalogo/tool registry disponibile;
4. ricostruisci permission e approval model;
5. registra il tool nel [Tool Risk Register](../examples/templates/tool-risk-register.csv);
6. collega l'analisi al capitolo [Tools & MCP](07-tools-mcp.md).

## Come adattare le query

### Segmentare per piattaforma

Aggiungi un filtro sulla piattaforma quando devi separare agenti Microsoft, custom o terze parti.

### Cercare record recenti

Aggiungi una finestra temporale coerente con il caso d'uso prima di confrontare due snapshot.

### Aggiungere campi

Usa lo schema explorer di Advanced Hunting e amplia il `project` solo con campi che risultano effettivamente presenti e valorizzati.

### Trasformare un finding in controllo

Non fermarti alla query.

Un finding utile deve diventare almeno una di queste cose:

- remediation;
- policy;
- ownership fix;
- approval requirement;
- monitoring rule;
- incident hypothesis;
- eccezione documentata.

## Evidence minima

Per un assessment salva:

- query eseguita;
- timestamp;
- tenant/environment;
- numero di record;
- screenshot o export consentito;
- finding sintetici;
- decisioni prese;
- link al backlog/remediation.

Evita di copiare dati sensibili in documentazione non protetta.

## Cosa leggere dopo

Se stai facendo un assessment:

1. [Registry & Governance](03-registry-governance.md)
2. [Defender](05-defender.md)
3. [Tools & MCP](07-tools-mcp.md)
4. [Tool governance runtime](tutorials/10-tool-governance-runtime.md)

Se stai investigando un incidente:

1. [Run Evidence & Correlation](tutorials/11-run-evidence-observability.md)
2. [Reliability & Incident Operations](tutorials/14-reliability-incident-operations.md)
3. [Agent Incident Runbook](../examples/checklists/agent-incident-runbook.md)

## Definition of Done

- [ ] schema `AgentsInfo` verificato nel tenant;
- [ ] inventory baseline prodotta;
- [ ] governance gap trasformati in backlog;
- [ ] candidati tool/MCP approfonditi;
- [ ] query e timestamp conservati come evidence;
- [ ] next action assegnata per ogni finding.
