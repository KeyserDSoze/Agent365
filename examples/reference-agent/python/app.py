"""
Agent 365 observability integration skeleton.

This sample deliberately keeps the "agent runtime" as a placeholder.
Replace run_agent() with your real LangChain / OpenAI Agents / Agent Framework /
Semantic Kernel or custom runtime.

For tenant export, replace token_resolver() with a supported OBO/S2S flow.
"""

import os
from microsoft.opentelemetry import use_microsoft_opentelemetry


def token_resolver(agent_id: str, tenant_id: str):
    # TODO: implement supported OBO or S2S token acquisition.
    # Returning None is safe for local console-only validation.
    return None


use_microsoft_opentelemetry(
    enable_a365=True,
    a365_token_resolver=token_resolver,
    enable_console=True,
)


def run_agent(user_input: str) -> str:
    # Replace with the actual agent framework call.
    return f"Reference agent received: {user_input}"


if __name__ == "__main__":
    print(run_agent(os.getenv("SAMPLE_PROMPT", "hello Agent 365")))
