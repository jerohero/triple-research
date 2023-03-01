using System.Text.RegularExpressions;

namespace CvWorker.Util; 

public static class StringUtil {
    public static string SpecialCharsToUnderscore(string s) {
        return Regex.Replace(s, "[|@#!¡·$€%&()=?¿^*;:,¨´><+]", "_");
    }
}