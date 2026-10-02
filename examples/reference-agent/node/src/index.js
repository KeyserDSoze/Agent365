import {
  useMicrosoftOpenTelemetry,
  shutdownMicrosoftOpenTelemetry,
} from "@microsoft/opentelemetry";

// Initialize before loading instrumented runtime libraries.
useMicrosoftOpenTelemetry({
  a365: {
    enabled: true,
    tokenResolver: async (agentId, tenantId) => {
      // TODO: implement supported OBO or S2S token acquisition.
      // null is appropriate only for local validation with Agent 365 export disabled.
      return null;
    },
  },
  enableConsoleExporters: true,
});

async function runAgent(userInput) {
  // Replace with your actual OpenAI Agents / LangChain / custom runtime call.
  return `Reference agent received: ${userInput}`;
}

try {
  console.log(await runAgent(process.env.SAMPLE_PROMPT ?? "hello Agent 365"));
} finally {
  await shutdownMicrosoftOpenTelemetry();
}
