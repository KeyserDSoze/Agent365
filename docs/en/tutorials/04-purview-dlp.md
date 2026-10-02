# Tutorial 04 — Purview DLP integration

## Goal
Insert a DLP decision into the agent flow before disallowed content reaches the model.

Current Agent 365 guidance documents the prompt:

> add Purview DLP to this agent

The generated path can use Microsoft Graph `processContent` checks for input blocking.

## Current prerequisites/caveats
Microsoft currently documents Purview DLP for AI enablement, relevant licensing, pay-as-you-go billing and DSPM for AI onboarding as prerequisites.

## Logical pattern

```text
Input → DLP processContent → blocked? stop/audit : agent/LLM → optional response audit → user
```

## Design decisions
Define what blocks, what only audits, fail-open vs fail-closed, timeout behavior, user messaging, telemetry and incident escalation.

## Test cases
Normal content, simulated sensitive content, timeout, authorization failure, response audit failure and bypass attempt.

Capture policy, expected result, actual result, audit evidence, timestamp and remediation.

Source: https://learn.microsoft.com/en-us/microsoft-agent-365/developer/get-started
