using Grammars.Enums;
using Grammars.Models.Person;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Text;





namespace Grammars.Contexts
{
  /// <summary>
  /// Контекст базы данных.
  /// </summary>
  public partial class GrammarsContext : DbContext
  {
    protected string _connString;
    protected bool _useSqlite = false;
    protected string _srvName = @"WS";
    protected string _dbName = @"Grammars";




    public GrammarsContext() : base()
    {
      _useSqlite = false;
      _connString = $"Data Source={_srvName};Initial Catalog={_dbName};Integrated Security=True;";
      Database.EnsureCreated();
    }
    public GrammarsContext(bool useSqlite) : base()
    {
      _useSqlite = useSqlite;
      _connString = useSqlite ? $"Data Source={_dbName}.db" : $"Data Source={_srvName};Initial Catalog={_dbName};Integrated Security=True;";
      Database.EnsureCreated();
    }
    public GrammarsContext(string serverName, string dbName, bool useSqlite = false) : base()
    {
      _srvName = serverName;
      _dbName = dbName;
      _useSqlite = useSqlite;
      _connString = $"Data Source={_srvName};Initial Catalog={_dbName};Integrated Security=True;";
      Database.EnsureCreated();
    }
    public GrammarsContext(DbContextOptions<GrammarsContext> options, string serverName, string dbName, bool useSqlite = false) : base(options)
    {
      _srvName = serverName;
      _dbName = dbName;
      _useSqlite = useSqlite;
      _connString = $"Data Source={_srvName};Initial Catalog={_dbName};Integrated Security=True;";
      Database.EnsureCreated();
    }








    public DbSet<PersonName> PersonNames { get; set; }
    public DbSet<GrammarCase> GrammarCases { get; set; }
    public DbSet<Grammar> Grammars { get; set; }










    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      if (!optionsBuilder.IsConfigured)
      {
        if (_useSqlite == true)
        {
          optionsBuilder.UseSqlite($"Data Source={_dbName}.db");
        }
        else
        {
          optionsBuilder.UseLazyLoadingProxies().UseSqlServer(_connString);
        };
      };
      optionsBuilder.EnableSensitiveDataLogging();
    }
    protected override void OnModelCreating(ModelBuilder mb)
    {
      mb.Entity<Grammar>(ent =>
      {
        ent.Property(e => e.For).HasConversion(new EnumToStringConverter<NamePart>());
      });
      mb.Entity<PersonName>(ent =>
      {
        ent.Property(e => e.Gender).HasConversion(new EnumToStringConverter<Gender>());
        ent.Property(e => e.Part).HasConversion(new EnumToStringConverter<NamePart>());
        ent.HasOne(e => e.Grammar).WithMany(w => w.Names).HasForeignKey(f => f.GrammarId).OnDelete(DeleteBehavior.ClientSetNull);
        ent.HasOne(e => e.BaseName).WithMany(w => w.Derived).HasForeignKey(f => f.BaseId).OnDelete(DeleteBehavior.Cascade);
      });
      mb.Entity<GrammarCase>(ent =>
      {
        ent.Property(e => e.Case).HasConversion(new EnumToStringConverter<Case>());
        ent.HasOne(e => e.Grammar).WithMany(w => w.GrammarCases).HasForeignKey(f => f.GrammarId).OnDelete(DeleteBehavior.Cascade);
      });
    }


  }
}
