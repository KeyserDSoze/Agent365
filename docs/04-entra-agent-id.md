# 04 — Microsoft Entra Agent ID

## Obiettivo
Rendere gli agenti identificabili, autenticabili, autorizzabili e governabili.

## Concetti
- **Agent identity blueprint:** definisce il modello con cui vengono create/governate identità.
- **Agent identity:** identità concreta usata dall'agente.
- **Sponsor/owner:** responsabilità e continuità.

## Pattern
- Interactive/on-behalf-of: contesto utente + agente.
- Autonomous: identità propria e autorizzazione dedicata.

## Principi
Identità unica, least privilege, niente credenziali condivise, owner esplicito, separazione ambienti, access review, credential lifecycle, revocation test.

## Conditional Access
Disegnare policy in base a tipo agente, risorsa, rischio, origine, auth flow e autonomia. Non clonare policy utente senza analisi.

## Identity Design Sheet
Documentare purpose, pattern, blueprint, identity, sponsor, credential model, token flow, scope, permission, CA, logging e revocation.

Template: `examples/templates/identity-design-sheet.md`.

## Fonti
- https://learn.microsoft.com/en-us/entra/agent-id/
- https://learn.microsoft.com/en-us/microsoft-agent-365/guidance/entra-agent-365
