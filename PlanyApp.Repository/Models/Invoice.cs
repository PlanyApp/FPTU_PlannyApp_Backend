using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlanyApp.Repository.Models;

[Table("Invoices")]
public partial class Invoice
{
    public int InvoiceId { get; set; }

    public int UserId { get; set; }
    public int UserId { get; set; }

    public int? PackageId { get; set; }
    public int? PackageId { get; set; }

    public DateTime InvoiceDate { get; set; }

    public DateTime? DueDate { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Amount { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Discount { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    [Column(TypeName = "decimal(19, 2)")]
    public decimal? FinalAmount { get; set; }

    public string Status { get; set; } = null!;

    public string? PaymentMethod { get; set; }

    public string? TransactionId { get; set; }

    public string? Notes { get; set; }
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? ReferenceCode { get; set; }

    public virtual Package? Package { get; set; }

    public virtual User User { get; set; } = null!;
}
