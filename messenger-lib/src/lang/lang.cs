

public enum LangCode {
    ru = 1,
    en = 2,
}
static class Lang {
    private static string[] sRu = new string[400];
    private static string[] sEn = new string[400];
    static string ConvertStatus(int status, LangCode lang) {
        try { 
            switch (lang) {
                case LangCode.ru: return sRu[status];
                case LangCode.en: return sEn[status];
                default: return "Translate error!";
            };
        } catch { return "Translate error!"; }
    }
    static Lang() {

    }
}