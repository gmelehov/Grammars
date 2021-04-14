using System;
using System.Collections.Generic;
using System.Text;



namespace Grammars.Enums
{
  /// <summary>
  /// Признак гендерной принадлежности.
  /// </summary>
  [Flags]
  public enum Gender
  {
    /// <summary>
    /// Не определен/не применим/не известен.
    /// </summary>
    N = 0,
    /// <summary>
    /// Женский гендер.
    /// </summary>
    F = 1,
    /// <summary>
    /// Мужской гендер.
    /// </summary>
    M = 2,

  }
}
