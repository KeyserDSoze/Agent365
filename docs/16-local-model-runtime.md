# 16 — Local Model Runtime: Microsoft Foundry Local

## Obiettivo

Separare il **model runtime** dal control plane Agent 365 e rendere i laboratori eseguibili senza dipendere da Azure OpenAI.

## Posizionamento

Per questo progetto il modello architetturale è:

```text
Agent Framework
     |
     v
Model Provider Boundary
     |
     +--> Foundry Local        ← default lab
     +--> Azure OpenAI         ← optional cloud provider
     +--> other compatible     ← future
```

Agent 365 continua a occuparsi di identity, observability, governance e security indipendentemente dal provider del modello.

## Perché Foundry Local

Microsoft Foundry Local offre:
- 20+ modelli locali pronti;
- catalogo gestito;
- alias hardware-aware;
- download/cache del modello;
- selezione automatica di varianti CPU/GPU/NPU;
- gestione execution provider;
- API OpenAI-compatible;
- SDK C#, Python, JavaScript e Rust;
- optional REST server.

Per il training questo riduce le dipendenze infrastrutturali e permette di sperimentare anche senza subscription Azure.

## Foundry Local vs Windows AI APIs vs Windows ML

### Windows AI APIs
Sono API Windows ad alto livello per capability AI pronte. Alcune richiedono Copilot+ PC o hardware specifico. Phi Silica è distinto da Foundry Local.

### Foundry Local
È la scelta raccomandata nel repository per LLM locali gestiti e portabili. Offre catalogo, runtime e API OpenAI-compatible.

### Windows ML
È il livello più flessibile per portare modelli ONNX propri e controllare direttamente l'inference stack.

## Agent Framework .NET: boundary OpenAI-compatible

La documentazione corrente di Agent Framework espone un `FoundryLocalClient` dedicato nel percorso Python; il supporto equivalente non è attualmente documentato per .NET.

Per questo il golden sample .NET usa:

```text
Agent Framework
    |
    v
Microsoft.Extensions.AI IChatClient
    |
    v
OpenAI SDK
    |
    v
Foundry Local /v1/chat/completions
```

Questo non è un workaround casuale: mantiene esplicito il model-provider boundary e permette di sostituire Foundry Local con Azure OpenAI senza modificare sessioni, tool o operating model dell'agente.

## SDK nativo

Il percorso Windows corrente usa:

```bash
dotnet add package Microsoft.AI.Foundry.Local.WinML
```

Il native SDK:
- inizializza il runtime in-process;
- individua il modello tramite alias;
- scarica il modello se non cached;
- carica il modello;
- espone un native chat client;
- può avviare un servizio REST locale.

Il quickstart Windows corrente richiede Windows 11 24H2+ e .NET 9+.

## CLI

Installazione:

```powershell
winget install Microsoft.FoundryLocal
```

Comandi principali:

```powershell
foundry model list
foundry model info phi-4-mini
foundry model download phi-4-mini
foundry model load phi-4-mini
foundry server restart --port 39839 --idle-timeout 0
foundry server status
```

La CLI è attualmente Preview.

## Alias vs model ID

Usare un **alias** quando si prepara il modello:

```text
phi-4-mini
```

Foundry Local seleziona una variante appropriata all'hardware.

Per il client OpenAI-compatible è preferibile risolvere il **model ID concreto caricato** attraverso `/v1/models`.

Il golden sample automatizza questa risoluzione.

## Privacy e boundary

In Foundry Local mode:
- l'inference viene eseguita sul dispositivo;
- non serve inviare il prompt ad Azure OpenAI;
- non serve una cloud model API key;
- la telemetry Agent 365, se abilitata, è un flusso separato e deve essere governato consapevolmente.

“Local model” non significa automaticamente “nessun dato lascia il device” se si abilitano telemetry, tool remoti o integrazioni cloud.

## Tool calling

Il supporto ai function tool dipende dal modello specifico. Il catalogo espone capability del modello; i lab che richiedono tool calling devono verificarne il supporto.

## Pattern di fallback

Un'applicazione reale può adottare:

```text
Foundry Local
     |
     +-- available / capable --> local inference
     |
     +-- unavailable ---------> Azure OpenAI / cloud fallback
```

Il fallback va reso esplicito per evitare che requisiti di privacy/offline vengano violati in modo trasparente.

## Golden sample

Vedi:
- `../samples/dotnet-golden-agent/`
- `tutorials/07-foundry-local.md`

Il provider predefinito è `foundry-local`; Azure OpenAI rimane selezionabile tramite configurazione.

## Fonti

- https://learn.microsoft.com/en-us/windows/ai/apis/local-llms
- https://learn.microsoft.com/en-us/windows/ai/foundry-local/get-started
- https://learn.microsoft.com/en-us/windows/ai/windows-ai-comparison
- https://learn.microsoft.com/en-us/azure/foundry-local/reference/reference-sdk-current
- https://learn.microsoft.com/en-us/azure/foundry-local/reference/reference-cli
- https://learn.microsoft.com/en-us/agent-framework/integrations/by-component/model-providers/foundry-local
