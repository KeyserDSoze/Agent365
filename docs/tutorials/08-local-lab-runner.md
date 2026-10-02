# Tutorial 08 — Local Lab Runner end-to-end

## Obiettivo

Eseguire un test ripetibile del golden agent usando **solo il PC Windows come model runtime** e produrre un'evidenza JSON del risultato.

Il runner verifica quattro livelli distinti:

1. Foundry Local installato e avviato;
2. modello scaricato/caricato e inference OpenAI-compatible diretta;
3. Golden Agent API avviata con Microsoft Agent Framework;
4. conversazione multi-turn sulla stessa `AgentSession`.

## Comando

Da PowerShell:

```powershell
cd samples/dotnet-golden-agent
.\scripts\run-local-lab.ps1
```

Default:

- model alias: `phi-4-mini`;
- Foundry Local: porta `39839`;
- Golden Agent: porta `5080`;
- Agent 365 cloud export: disabilitato;
- console telemetry: abilitata.

## Cosa succede

### Step 1 — Foundry Local bootstrap

Il runner richiama `start-foundry-local.ps1`.

Lo script:
- verifica la CLI;
- prova a installarla via winget se manca;
- avvia il daemon;
- scarica e carica il modello;
- risolve il model ID concreto;
- configura il provider del golden agent.

### Step 2 — Inference diretta

Prima di coinvolgere Agent Framework viene chiamato direttamente:

```text
POST http://127.0.0.1:39839/v1/chat/completions
```

Questo separa un eventuale problema del **model runtime** da un problema dell'**agent runtime**.

### Step 3 — Golden Agent

Il runner avvia:

```text
dotnet run
```

e aspetta:

```text
GET /health
```

Poi legge:

```text
GET /api/config
```

per verificare che il provider reale sia `foundry-local` e che il model ID coincida con quello caricato.

### Step 4 — Primo turno

Viene chiamato:

```text
POST /api/chat
```

Il test fallisce se la risposta è vuota.

### Step 5 — Multi-turn

Il runner riusa il `conversationId` del primo turno.

Il test fallisce se:
- il secondo output è vuoto;
- il conversation ID cambia;
- l'API non risponde.

## Output

I risultati vengono scritti in:

```text
samples/dotnet-golden-agent/lab-output/
```

File:

```text
latest.json
lab-YYYYMMDD-HHMMSS.json
agent-stdout.log
agent-stderr.log
```

La cartella è esclusa da Git.

## Esempio report

```json
{
  "provider": "foundry-local",
  "modelAlias": "phi-4-mini",
  "modelId": "<resolved-id>",
  "checks": {
    "foundryPrepared": true,
    "directInference": true,
    "agentHealth": true,
    "safeConfig": true,
    "firstTurn": true,
    "multiTurn": true
  },
  "success": true
}
```

## Parametri utili

Cambiare modello:

```powershell
.\scripts\run-local-lab.ps1 -ModelAlias qwen2.5-0.5b
```

Cambiare porte:

```powershell
.\scripts\run-local-lab.ps1 -FoundryPort 39900 -AgentPort 5090
```

Lasciare l'agent avviato dopo il test:

```powershell
.\scripts\run-local-lab.ps1 -KeepRunning
```

Fermare anche Foundry Local alla fine:

```powershell
.\scripts\run-local-lab.ps1 -StopFoundryOnExit
```

## Troubleshooting

### Direct inference fallisce

Controllare:

```powershell
foundry server status
foundry model list --loaded --verbose
foundry server logs
```

### Agent health passa ma /api/chat fallisce

Controllare:
- `Agent__Provider`;
- `Agent__FoundryLocalEndpoint`;
- `Agent__Model`;
- `lab-output/agent-stderr.log`.

### Il modello risponde ma non usa i tool

Non tutti i modelli supportano function calling.

Verificare le capability del modello nel catalogo. Il lab base non richiede tool calling proprio per poter distinguere la validazione del runtime dalla validazione delle capability avanzate.

## Definition of Done

- [ ] `success=true` nel report;
- [ ] inference diretta locale non vuota;
- [ ] health API 200;
- [ ] provider `foundry-local`;
- [ ] primo turno Agent Framework non vuoto;
- [ ] secondo turno sullo stesso conversation ID;
- [ ] report archiviato come evidenza del lab.

## Fonti

- https://learn.microsoft.com/en-us/azure/foundry-local/how-to/how-to-use-foundry-local-cli
- https://learn.microsoft.com/en-us/azure/foundry-local/reference/reference-rest
- https://learn.microsoft.com/en-us/windows/ai/foundry-local/get-started
