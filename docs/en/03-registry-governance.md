# 03 — Agent Registry, Inventory & Governance

## Principle
You cannot govern what you cannot see. Every assessment starts with a baseline.

## Agent Overview
Current Microsoft guidance describes agent count, recent activity, usage trends, governance gaps, approval requests and ownerless agents.

## Agent Registry
The logical source of truth for agents visible in the tenant. Copilot Studio agents are automatically registered according to current documentation.

## Minimum inventory
Name/ID, platform, type, owner, sponsor, status, identity, audience, data sources, tools/MCP, permissions, environment, risk tier, lifecycle, telemetry and review date.

Template: `../../examples/templates/agent-inventory.csv`.

## Lifecycle
**Discover → Classify → Assign ownership → Approve → Publish → Monitor → Review → Retire**

## Governance gaps
Missing owner, undefined identity, high-impact tools, undocumented data access, excessive audience, stale agents, unclassified third-party agents and insufficient telemetry.

## Internal risk tiers
- Low: read-only, non-sensitive data.
- Medium: business data, operational tools, reversible impact.
- High: write actions, sensitive data, critical systems or high autonomy.

This is an internal operating model, not a Microsoft standard.
