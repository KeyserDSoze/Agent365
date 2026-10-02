# Tutorial 07 — Foundry Local come model runtime

## Perché

Per i lab Agent 365 non è necessario consumare Azure OpenAI. Microsoft Foundry Local permette di eseguire modelli LLM direttamente sul PC, con catalogo gestito, scelta automatica della variante hardware e API OpenAI-compatible.

Per il nostro golden sample questa è la modalità **predefinita**.

## Cosa gestisce Foundry Local

- catalogo di 20+ modelli OSS;
- download del modello;
- cache locale;
- scelta della variante compatibile con CPU/GPU/NPU;
- execution provider;
- caricamento/scaricamento del modello;
- endpoint locale OpenAI-compatible.

## Installazione Windows

Il nostro script la esegue automaticamente quando possibile:

```powershell
cd samples/dotnet-golden-agent
.\scripts\start-foundry-local.ps1 -RunAgent
```

Se `foundry` non è presente, lo script usa:

```powershell
winget install Microsoft.FoundryLocal
```

La CLI è attualmente Preview; l'SDK nativo è il percorso da preferire quando Foundry Local viene incorporato direttamente in un'applicazione.

## Modello predefinito

Il lab usa l'alias:

```text
phi-4-mini
```

L'alias è preferibile a un model ID hard-coded perché Foundry Local può scegliere la variante più adatta all'hardware disponibile.

Il catalogo cambia nel tempo; verificare sempre:

```powershell
foundry model list
foundry model info phi-4-mini
```

## Cosa fa lo script

1. verifica se Foundry Local è installato;
2. se manca, prova a installarlo via winget;
3. avvia/restarta il server locale sulla porta 39839;
4. scarica `phi-4-mini` se necessario;
5. carica il modello;
6. interroga `/v1/models` per ottenere il model ID reale;
7. imposta:
   - `Agent__Provider=foundry-local`;
   - `Agent__FoundryLocalEndpoint=http://127.0.0.1:39839/v1`;
   - `Agent__Model=<resolved model id>`;
8. opzionalmente avvia il golden agent.

## Architettura

```text
Golden Agent API
      |
      v
Microsoft Agent Framework
      |
      v
OpenAI-compatible client
      |
      v
http://127.0.0.1:39839/v1
      |
      v
Foundry Local
      |
      +--> catalog
      +--> model cache
      +--> CPU/GPU/NPU execution provider
      +--> local LLM
```

## Nessuna Azure credential

In modalità Foundry Local:
- non serve `az login`;
- non serve Azure OpenAI;
- non serve una API key cloud;
- prompt e inference restano sul device.

Agent 365 observability rimane indipendente dal model provider: si può tenere solo console oppure abilitarne l'export S2S.

## Tool calling

La disponibilità del tool calling dipende dal modello selezionato. Prima di usare i function tool del golden agent in un test completo, verificare che il modello scelto dichiari supporto tool calling.

## Alternative Windows

Microsoft distingue:
- Windows AI APIs: modelli/API pronti, spesso legati a specifico hardware;
- Foundry Local: LLM locali gestiti e OpenAI-compatible;
- Windows ML: bring-your-own ONNX con controllo più basso livello.

Per questo repository, Foundry Local è il compromesso migliore tra semplicità e controllo.

## Fonti

- https://learn.microsoft.com/en-us/windows/ai/apis/local-llms
- https://learn.microsoft.com/en-us/windows/ai/foundry-local/get-started
- https://learn.microsoft.com/en-us/azure/foundry-local/reference/reference-cli
- https://learn.microsoft.com/en-us/azure/foundry-local/reference/reference-sdk-current
