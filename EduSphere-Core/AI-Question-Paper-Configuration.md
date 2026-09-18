# AI Question Paper Configuration

## Database

Apply migration `20260918213000_AIQuestionPaperGeneration` or run:

```text
DatabaseScripts/20260918_AIQuestionPaperGeneration_Promotion.sql
```

The promotion script creates the six AI workflow tables and inserts a version-one general prompt template for every existing tenant. It does not insert provider credentials.

## Data Protection

AI API keys are encrypted with ASP.NET Core Data Protection before they are persisted. In UAT and production, configure `DataProtection:KeysPath` to durable storage shared by every application instance. Back up and protect that key ring; losing it makes stored provider credentials unreadable.

## Provider Setup

1. Sign in as SuperAdmin or TenantAdmin and select the tenant.
2. Open **AI Papers**.
3. Under **Configuration**, choose OpenAI or Azure OpenAI.
4. Enter the endpoint, model/deployment, API key, region, residency policy, token pricing and timeout.
5. Save the provider and create additional prompt-template versions when assessment policy changes.

For OpenAI, an endpoint ending in `/v1`, `/chat/completions`, or the service root is accepted. For Azure OpenAI, enter the resource endpoint; EduSphere adds the deployment and API-version path. A complete chat-completions URL is also accepted.

## Security And Governance

- Provider keys are never returned to the UI or API.
- Generation prompts are scrubbed for email addresses, phone numbers and 12-digit identity numbers.
- A tenant can require a provider region or block external processing with `LocalOnly`.
- Generated output must pass structure, marks-total and duplicate-question validation.
- Every successful run records tokens, estimated cost, latency, model and provider request ID.
- Provider failures can create a manual draft without exposing credentials or blocking paper preparation.
- AI questions enter the reusable question bank only after the paper and selected version are approved.
