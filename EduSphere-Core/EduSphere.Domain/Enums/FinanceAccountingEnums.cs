namespace EduSphere.Domain.Enums;

public enum AccountType { Asset=1, Liability=2, Equity=3, Income=4, Expense=5 }
public enum JournalStatus { Draft=1, Posted=2, Reversed=3 }
public enum VoucherType { Receipt=1, Payment=2, Journal=3, Contra=4, CreditNote=5, DebitNote=6 }
public enum AccountingPeriodStatus { Open=1, Closed=2 }
public enum BankStatementStatus { Unmatched=1, Matched=2, Ignored=3 }
public enum InstallmentStatus { Scheduled=1, Invoiced=2, Paid=3, Overdue=4, Waived=5 }
public enum LateFeeMethod { Fixed=1, Percentage=2, PerDay=3 }
public enum TaxType { None=0, Gst=1, Cgst=2, Sgst=3, Igst=4 }
public enum GatewayWebhookStatus { Received=1, Verified=2, Processed=3, Rejected=4, Failed=5 }
