# Tutorial 09 — Hardening operativo del Golden Agent

## Obiettivo

Portare il golden sample dal livello **lab** al livello **POC controllato**, mantenendo configurabili i guardrail.

## Health vs readiness

### `GET /health`

Risponde se il processo ASP.NET Core è vivo.

Non garantisce che:
- il modello sia configurato;
- Foundry Local sia avviato;
- Azure OpenAI sia raggiungibile.

### `GET /ready`

Verifica il model provider.

Per `foundry-local`:
- model ID configurato;
- endpoint valido;
- `/openai/status` raggiungibile.

Per `azure-openai`:
- endpoint configurato;
- host raggiungibile.

Model/auth vengono comunque validati definitivamente alla prima invocation.

## API key opzionale

Default lab:

```text
Api__RequireApiKey=false
```

POC:

```powershell
$env:Api__RequireApiKey="true"
$env:Api__ApiKey="<secret>"
```

Gli endpoint operativi protetti sono:
- `POST /api/chat`;
- `DELETE /api/conversations/{id}`;
- `GET /api/diagnostics`.

Header:

```text
X-Api-Key: <secret>
```

L'API key non compare in `/api/config`.

> È un guardrail semplice per lab/POC. Non sostituisce Entra ID, reverse proxy o API Management in produzione.

## Rate limiting

Default:

```text
Api__RequestsPerMinute=30
```

Il limite è applicato a `POST /api/chat`.

Superata la soglia:

```text
HTTP 429
```

con risposta JSON `rate_limit_exceeded`.

## Limiti input

Default:

```text
Api__MaxMessageCharacters=8000
Api__MaxConversationIdCharacters=128
```

Un payload troppo grande viene rifiutato prima dell'inference.

Questo riduce:
- consumo accidentale;
- prompt oversized;
- uso improprio del conversation ID;
- superficie di abuso.

## Conversation store

Il sample usa memoria in-process.

Guardrail:

```text
Conversations__MaxConversations=200
Conversations__IdleTimeoutMinutes=30
```

Le sessioni idle vengono rimosse automaticamente.

Se la capacità è raggiunta:

```text
HTTP 429
conversation_capacity_reached
```

Per produzione serve uno storage/lifecycle strategy adeguato al framework e ai requisiti di continuità.

## Diagnostics

```text
GET /api/diagnostics
```

Restituisce:
- readiness;
- provider/model;
- numero conversazioni attive;
- sessioni idle rimosse;
- timestamp.

Non restituisce secret.

Se API key è abilitata, l'endpoint è protetto.

## Configurazione POC suggerita

```text
Api__RequireApiKey=true
Api__RequestsPerMinute=20
Api__MaxMessageCharacters=6000
Conversations__MaxConversations=100
Conversations__IdleTimeoutMinutes=20
```

Il valore reale dipende da use case, modello, hardware e carico.

## Test automatici

La repository esegue test HTTP per verificare:
- health 200;
- readiness 503 quando il modello non è configurato;
- secret non esposti;
- API key;
- payload limit;
- rate limit;
- conversation capacity.

## Definition of Done

- [ ] `/health` e `/ready` hanno semantica distinta;
- [ ] secret assenti da `/api/config`;
- [ ] API key testata se richiesta;
- [ ] rate limit testato;
- [ ] max input testato;
- [ ] conversation capacity definita;
- [ ] diagnostics accessibile solo secondo il security model;
- [ ] test automatici verdi.
