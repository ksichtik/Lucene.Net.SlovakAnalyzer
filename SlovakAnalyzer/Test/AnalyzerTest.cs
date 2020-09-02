using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Tokenattributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace SLovakAnalyzerTest
{
    public class AnalyzerTest
    {
        [Fact]
        public void Slovak_analyzer_test()
        {
            var inputString = "Keďže poľnohospodári Finstatu, Chirany, Lega, U. S. Steelu Košice a dubu z finstat.sk v Zambii preferujú skôr všestranné a spoľahlivé traktory s ľahkou údržbou...";
            var expectedString = "keďže poľnohospodár finstat chiran lego u s steel košic a dub z finstat.sk v zambi preferovať skôr všestranný a spoľahlivý traktor s ľahký údržba";
            List<string> tokens = new List<string>();
            var analyzer = new SlovakAnalyzer.SlovakAnalyzer();
            var tokenStream = analyzer.TokenStream(null, new StringReader(inputString));
            var offsetAttribute = tokenStream.GetAttribute<IOffsetAttribute>();
            var termAttribute = tokenStream.GetAttribute<ITermAttribute>();

            tokenStream.Reset();
            while (tokenStream.IncrementToken())
            {
                int startOffset = offsetAttribute.StartOffset;
                int endOffset = offsetAttribute.EndOffset;
                String term = termAttribute.Term;
                tokens.Add(term);
            }

            Assert.Equal(expectedString, string.Join(" ", tokens));
        }

        [Fact]
        public void Slovak_analyzer_long_text_test()
        {
            var inputString = "Daniel HORŇÁK, moderátor -------------------- Slovenské hospodárstvo vzrástlo podľa rýchleho odhadu Štatistického úradu v prvom štvrťroku o viac než tri percenta. Potvrdzujú sa tak pôvodné odhady analytikov i predpoklady jarnej prognózy Európskej komisie. Čo tento vývoj znamená pre firmy, obyvateľov a štát, o tom sa budeme rozprávať v nasledujúcich minútach. Pozvanie do štúdia prijala Katarína Muchová, analytička Slovenskej sporiteľne, dobrý deň, vitajte. Katarína MUCHOVÁ, analytička Slovenskej sporiteľne -------------------- Dobrý deň, ďakujem za pozvanie. Daniel HORŇÁK, moderátor -------------------- Keby ste mohli na úvod povedať, ako hodnotíte tie zverejnené údaje Štatistického úradu za prvý kvartál v kontexte vlastne celého vývoja, ktorý sa očakáva pre rok 2017? Katarína MUCHOVÁ, analytička Slovenskej sporiteľne -------------------- Výsledok bol podľa nás očakávaný, pretože náš odhad rátal s rastom ekonomiky o 3,1 percenta v medziročnom porovnaní. Čiže v porovnaní s prvým kvartálom minulého roka. Takže dopadlo to vlastne viacmenej ako sme čakali. Na štruktúru si ale musíme ešte počkať, tá bude známa až o mesiac neskôr, čiže v júni. Celkovo však vidíme, že podľa tých mesačných údajov, ktoré boli priebežne zverejňované, napríklad cez maloobchodné tržby, údaje zahraničného obchodu, priemyselnej produkcie či stavebníctva, tak vidno tam veľmi pekne, že veľa do toho určite vstúpila domáca spotreba, spotreba domácností, tá pravdepodobne bola hlavným ťahúňom ekonomiky, ale takisto tam vidno aj, že celkom slušne sa darilo zahraničnému obchodu, takže aj čisté vývozy by mali pozitívne prispievať k rastu HDP. Daniel HORŇÁK, moderátor -------------------- Slovenská ekonomika je známa tým, že dlhé roky bola vyslovene závislá od zahraničného dopytu, teda exportu. Aký podiel teda v súčasnosti zastáva táto časť? Katarína MUCHOVÁ, analytička Slovenskej sporiteľne -------------------- Minulý rok to bolo skoro vyrovnané. V istých kvartáloch dokonca zahraničný obchod, teda v čistom vyjadrení očistený o dovozy dokonca predbiehal aj spotrebu z domácej ekonomiky, čiže domácností, investície, vládnu spotrebu. Takže teraz vidíme, že zatiaľ je to v podstate vyrovnané, ale očakáva sa, že /nezrozumiteľné/ málo si udržať spotreba domácností a až potom neskôr by sa tam mali pripájať čisté vývozy. Čo sa týka ale ďalších prvkov zo štruktúry, tak ešte vieme, že vlani vlastne verejné investície mali negatívne medziročné rasty, práve pretože tam bol ten veľký bázický efekt z roku 2015, keď sa vo výraznej miere dočerpávali eurofondy z predchádzajúceho programového obdobia. Teraz už tento efekt vyprchal, takže postupne by sa mali aj do rastu vrátiť investície, či už cez verejné alebo súkromné investície, ktoré majú veľmi dobré podmienky na rast. Daniel HORŇÁK, moderátor -------------------- Poďme sa teraz pozrieť na tú ďalšiu časť a to je domáci dopyt. Jednou dôležitou zložkou je práve spotreba domácností. Je to určite dobré v rámci ekonomiky, keď aj domácnosti spotrebovávajú výrobky, služby, tovary, ktoré sú produkované v ekonomike. Čo teda spôsobuje, že v uplynulom období tá spotreba rastie na Slovensku? Katarína MUCHOVÁ, analytička Slovenskej sporiteľne -------------------- Darí sa domácnostiam aj preto, že na trhu práce sa situácia výrazne zlepšila. Vidíme vlastne už posledné dva - tri roky výrazné poklesy v miere nezamestnanosti a do veľkej miery je to spôsobené práve tvorbou nových pracovných miest. Čiže viac ľudí si nájde prácu a takisto potom aj sa darí rastu miezd, či už v reálnom alebo nominálnom vyjadrení. A toto všetko prispieva k tomu, že disponibilné prostriedky domácností sú vyššie. Takže keď sa disponibilný príjem zvýšil, potom môže byť tento dodatočný príjem buď investovaný do spotrebného tovaru, dlhšieho tovaru alebo aj teda nejako daný do úspor. Daniel HORŇÁK, moderátor -------------------- Ak by sme sa pozreli na celkový obraz, čo vlastne spôsobuje to, že táto situácia a ten trend je taký pozitívny, že pribúdajú voľné pracovné miesta, vytvárajú sa teda pracovné miesta pre ľudí? Katarína MUCHOVÁ, analytička Slovenskej sporiteľne -------------------- Do istej miery to ide ruka v ruke s tým ako sa darí hospodárstvu. Čiže celkovo rast slovenskej ekonomiky tým, že sa nám darí už niekoľko rokov za sebou, tak sa to prejavuje aj na trhu práce, ktorý už na toto oživenie čakal. Čiže s istým opozdením sa vlastne začala znižovať miera nezamestnanosti po kríze s tým, že k rastu HDP sme sa vrátili pomerne rýchlo, ale na trhu práce to trvalo ešte o niekoľko rokov dlhšie a až teraz v poslednom období vidíme naozaj veľmi rázne poklesy, už sa blížime k tým historicky najnižším hodnotám miery nezamestnanosti. Takže to je pozitívna správa. A druhým faktorom je tiež vývoj externého prostredia. Čiže celkovo sentiment ekonomická dôvera v Európe sa zlepšujú, už po kríze isté obavy opadli a tak aj zamestnávatelia vytvárajú nové pracovné miesta. Ekonomike sa darí. Daniel HORŇÁK, moderátor -------------------- Vyhliadky sú teda pozitívne. Čo sa týka rizík, ako hodnotíte tie faktory, ktoré môžu vstúpiť v najbližších mesiacoch a negatívne práve ovplyvniť tento vývoj, ktorý je očakávaný? Katarína MUCHOVÁ, analytička Slovenskej sporiteľne -------------------- My očakávame rast ekonomiky v tomto roku na úrovni 3,1 percenta a rizika vidíme skôr umiernené. Takže momentálne nie sú tam nejaké žiadne veľké hrozby, ktoré by mohli našu ekonomiku ohroziť, či už z domáceho alebo z vonkajšieho prostredia. Do istej miery samozrejme stále treba brať ohľad na Brexit a vyjednávania o Brexite, ktoré môžu zatriasť eurom alebo ekonomickým sentimentom, ale veľké výkyvy sa v tomto zmysle neočakávajú. Čo sa týka prezidentských volieb vo Francúzsku, tak to bolo predtým riziko, ale dopadlo v prospech trhov, takže protrhové reformy, ktoré sa očakávajú vo Francúzsku by mali práve pomôcť aj čo sa týka celkovej pozície eurozóny aj pozície eura voči ostatným menám. A z ostatných faktorov, tak stále tam môže byť rast eurozóny, prípadne globálny rast, ktorý môže byť krehkejší ako sa očakával, ale je to skôr umiernené riziko. Daniel HORŇÁK, moderátor -------------------- Prečo sú tie riziká miernejšie ako pred napríklad rokom - dvomi. Vieme napríklad, že teda Brexit stále bude, budú predčasné voľby pravdepodobne v Británii. Čiže prečo toto riziko napríklad je menšie ako pred niekoľkými mesiacmi, podobne napätie v rámci reforiem, očakávania v rámci Európskej únie, eurozóny. Čiže tieto riziká stále sú tu, prečo sú podľa vás teda o niečo miernejšie ako pred pár mesiacmi? Katarína MUCHOVÁ, analytička Slovenskej sporiteľne -------------------- Ešte pred pár mesiacmi sme hlavne nevedeli výsledok francúzskych prezidentských volieb a bolo tam riziko, že sa k moci dostanú teda skôr protieurópske názory a to by výrazne otriaslo nielen eurom, ale aj celkovo ekonomickým sentimentom v eurozóne. Takže víťazstvo Macrona bolo trhmi veľmi vítané. Ďalším rizikom, ktoré ešte pred pár mesiacmi sme sledovali, tak boli aj americké prezidentské voľby, ten, že už vlastne tým, že je výsledok známy a zatiaľ nedošlo k nejakým výrazným ekonomickým zmenám v americkej ekonomike, tak to trhy ukľudnilo. A ďalšie riziká by ešte by som spomenula, prípadne tie nemecké voľby, ktoré nás čakajú na jeseň, ale tam prieskumy ukazujú skôr že by mal teda nemalo by prísť k nejakým prekvapeniam, malo by prísť k potvrdeniu pozície súčasnej politiky Nemecka. Takže aj z tohto dôvodu vidíme, že tie riziká sa umierňujú. Daniel HORŇÁK, moderátor -------------------- Na záver, ak by ste teda mali zhrnúť ten výhľad pre slovenskú ekonomiku, vy ste teda spomenuli, že očakávate ten hospodársky rast nad úrovňou troch percent, čo sa týka ale napríklad trhu práce, kam sa až môže dostať miera nezamestnanosti, či ten vývoj by mal ísť tak, ako ho sledujeme v posledných mesiacoch až rokoch? Katarína MUCHOVÁ, analytička Slovenskej sporiteľne -------------------- Miera nezamestnanosti klesá už dlhšie obdobie a my očakávame, že v priemere bude tento rok ešte nižšia ako bola vlani. Celkový odhad na priemer roka máme na úrovni 8,7 percenta, čo vychádza z tých kvartálnych čísiel Štatistického úradu, čiže nie tých mesačných zverejňovaných cez úrady práce, ale celkovo ten hlavný faktor, ktorý bude znižovať mieru nezamestnanosti by mal byť naďalej zamestnanosť. Takže rast zamestnanosti, tvorba nových pracovných miest a to je veľkým pozitívom. Daniel HORŇÁK, moderátor -------------------- Pani Muchová, v tejto chvíli vám veľmi pekne ďakujem za zaujímavé informácie a prajem vám príjemný zvyšok dňa. Katarína MUCHOVÁ, analytička Slovenskej sporiteľne -------------------- Ďakujem.";
            var expectedString = "";
            List<string> tokens = new List<string>();
            var analyzer = new SlovakAnalyzer.SlovakAnalyzer();
            var tokenStream = analyzer.TokenStream(null, new StringReader(inputString));
            var offsetAttribute = tokenStream.GetAttribute<IOffsetAttribute>();
            var termAttribute = tokenStream.GetAttribute<ITermAttribute>();

            tokenStream.Reset();
            while (tokenStream.IncrementToken())
            {
                int startOffset = offsetAttribute.StartOffset;
                int endOffset = offsetAttribute.EndOffset;
                String term = termAttribute.Term;
                tokens.Add(term);
            }

            Assert.NotEqual(expectedString, string.Join(" ", tokens));
        }

        [Fact]
        public void Slovak_analyzer_parsequerystring_test()
        {
            var inputString = "Finstat Finstate Finstatu Finstatom Baťa Baťu Chirana Chirane Rajo a.s. Rajom";
            var expectedString = "finstat finstat finstat finstat bať bať chiran chiran raj as raj";
            List<string> tokens = new List<string>();
            var analyzer = new SlovakAnalyzer.SlovakAnalyzer();
            var outputString = analyzer.ParseQueryString(inputString);

            Assert.Equal(expectedString, outputString);
        }

        [Fact]
        public void Slovak_noun_analyzer_test()
        {
            var inputString = "Keďže najväčšia poľnohospodári Finstatu, Chirany, Lega, U. S. Steelu Košice a dubu z finstat.sk v Zambii preferujú skôr všestranné a spoľahlivé traktory s ľahkou údržbou...";
            var expectedString = "keďž najväčši poľnohospodár finstat chiran leg u steel košic dub finstat.sk zambi preferujú skôr všestranné spoľahlivé traktor ľahk údržb";
            List<string> tokens = new List<string>();
            var analyzer = new SlovakAnalyzer.SlovakNounAnalyzer();
            var tokenStream = analyzer.TokenStream(null, new StringReader(inputString));
            var offsetAttribute = tokenStream.GetAttribute<IOffsetAttribute>();
            var termAttribute = tokenStream.GetAttribute<ITermAttribute>();

            tokenStream.Reset();
            while (tokenStream.IncrementToken())
            {
                int startOffset = offsetAttribute.StartOffset;
                int endOffset = offsetAttribute.EndOffset;
                String term = termAttribute.Term;
                tokens.Add(term);
            }

            Assert.Equal(expectedString, string.Join(" ", tokens));
        }
    }
}