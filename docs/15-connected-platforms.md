# 15 — Connected Platforms

## Obiettivo
Portare agenti eseguiti su piattaforme esterne dentro il Registry di Microsoft Agent 365 per ottenere visibilità e governance centralizzate.

## Flusso amministrativo
Percorso documentato corrente:
**Microsoft 365 admin center → Agents → All Agents → Connected platforms → Manage → Connect a platform**.

La connessione viene autenticata verso l'ambiente esterno; gli agenti vengono sincronizzati nel Registry. L'amministratore può verificare stato, ultimo sync, errori e totale agenti sincronizzati.

## Piattaforme supportate — stato verificato 2026-10-02
La documentazione Microsoft aggiornata al 15 settembre 2026 elenca:
- Amazon Bedrock
- Google Vertex AI
- Salesforce Agentforce
- Databricks Genie
- Anthropic Claude Managed Agents
- Oracle Generative AI Agents
- Snowflake Cortex

L'integrazione Anthropic Claude Managed Agents è indicata come **Preview** perché le relative API sono in beta.

## Security considerations
Per ogni connection:
- usare credential dedicate;
- applicare least privilege;
- documentare scope e region;
- definire rotation/revocation;
- verificare chi può creare o cancellare agenti tramite le API esterne;
- preferire account/workspace dedicati quando raccomandato;
- monitorare sync error e stale connection.

## Discovery output
Per ogni piattaforma connessa documentare:
- provider;
- environment/workspace;
- region;
- auth method;
- credential owner;
- permissions;
- sync mode;
- number of agents;
- supported actions;
- revocation path;
- preview limitations.

## Fonte
https://learn.microsoft.com/en-us/microsoft-agent-365/admin/connected-platforms
