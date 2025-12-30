using System.Collections.Generic;
using System.Linq;


// ReSharper disable UnusedMember.Global

namespace Oxide.Plugins;

public partial class UiBuilderLibrary
{
    /// <summary>
    /// Transpose an enumable of 2-tuples into a 2-tuple of enumables.
    /// </summary>
    internal static (IEnumerable<T1>, IEnumerable<T2>) Transpose<T1, T2>(IEnumerable<(T1, T2)> source)
    {
        var valueTuples = source as (T1, T2)[] ?? source.ToArray();
        var rows = valueTuples.Length;
        var result = (new T1[rows], new T2[rows]);

        var index = 0;
        foreach (var item in valueTuples)
        {
            result.Item1[index] = item.Item1;
            result.Item2[index] = item.Item2;
            index += 1;
        }

        return result;
    }

    /// <summary>
    /// Transpose an enumable of enumables of 2-tuples then flatten each enumable in the resulting 2-tuple.
    /// </summary>
    internal static (IEnumerable<T1>, IEnumerable<T2>) TransposeAndFlatten<T1, T2>(
        IEnumerable<(IEnumerable<T1>, IEnumerable<T2>)> source)
    {
        var (transposedT1, transposedT2) = Transpose(source);
        var flattenT1 = transposedT1.SelectMany(data => data);
        var flattenT2 = transposedT2.SelectMany(data => data);
        return (flattenT1, flattenT2);
    }
}