using Grammars.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;




namespace Grammars.Models.Person
{
  /// <summary>
  /// Правило, описывающее возможную комбинацию значимых
  /// частей имени для конкретного падежа.
  /// <para />
  /// Составные части имени/отчества:
  /// [начальная часть имени][суффикс][окончание]
  /// <para />
  /// Любое имя, неизменяемая часть которого оканчивается
  /// на [суффикс], в указанном падеже будет иметь указанное [окончание]
  /// <para />
  /// [...][м|т|в]а   ==>   ОН [Кузь][м]а,   ОТ [Кузь][м]ы,   ДАЮ [Кузь][м]е,    ПРО [Кузь][м]у,    С [Кузь][м]ой,    О [Кузь][м]е
  /// [...][м|т|в]а   ==>   ОН  [Сав][в]а,   ОТ  [Сав][в]ы,   ДАЮ  [Сав][в]е,    ПРО  [Сав][в]у,    С  [Сав][в]ой,    О  [Сав][в]е
  /// 
  /// [...][к|г]а     ==>   ОНА [Оль][г]а,   ОТ [Оль][г]и,    ДАЮ [Оль][г]е,     ПРО [Оль][г]у,     С [Оль][г]ой,     ОБ [Оль][г]е
  /// </summary>
  public class GrammarCase
  {
    public GrammarCase()
    {

    }
    public GrammarCase(Grammar grammar) : this()
    {
      Grammar = grammar;
      GrammarId = grammar.Id;
    }
    public GrammarCase(Grammar grammar, XElement xelem) : this(grammar)
    {
      if(xelem != null)
      {
        switch (xelem.Name.LocalName)
        {
          case "Nom": Case = Case.NOM; break;
          case "Gen": Case = Case.GEN; break;
          case "Dat": Case = Case.DAT; break;
          case "Acc": Case = Case.ACC; break;
          case "Ins": Case = Case.INS; break;
          case "Loc": Case = Case.LOC; break;

          default: Case = Case.NOM; break;
        };

        MForm = xelem.Attribute("m")?.Value;
        FForm = xelem.Attribute("f")?.Value;
      };
    }













    /// <summary>
    /// Identity.
    /// </summary>
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// Падеж.
    /// </summary>
    [Required, MaxLength(3)]
    public Case Case { get; set; }

    /// <summary>
    /// Изменяемое по падежам окончание для мужской гендерной формы.
    /// </summary>
    [MaxLength(20)]
    public string MForm { get; set; }

    /// <summary>
    /// Изменяемое по падежам окончание для женской гендерной формы.
    /// </summary>
    [MaxLength(20)]
    public string FForm { get; set; }









    [NotMapped]
    public string CaseRus
    {
      get
      {
        switch (Case)
        {
          case Case.NOM: return "1 - именительный (кто?)";
          case Case.GEN: return "2 - родительный (кого?)";
          case Case.DAT: return "3 - дательный (кому?)";
          case Case.ACC: return "4 - винительный (кого?)";
          case Case.INS: return "5 - творительный (с кем?)";
          case Case.LOC: return "6 - предложный (о ком?)";

          default: return "";
        };
      }
    }








    /// <summary>
    /// Внешний ключ.
    /// </summary>
    public int GrammarId { get; set; }

    /// <summary>
    /// Ссылка на родительскую грамматику, в рамках которой определено это правило.
    /// </summary>
    [ForeignKey("GrammarId")]
    public virtual Grammar Grammar { get; set; }

  }
}
