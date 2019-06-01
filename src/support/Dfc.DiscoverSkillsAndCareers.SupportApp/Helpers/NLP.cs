using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using com.sun.corba.se.impl.encoding;
using edu.stanford.nlp.ling;
using edu.stanford.nlp.pipeline;
using java.util;
using Newtonsoft.Json;
using Syn.WordNet;

namespace Dfc.DiscoverSkillsAndCareers.SupportApp
{
    
    public class NLP
    {
        public class Document
        {
            public string Key { get; }
            public IDictionary<Term, double> TermFrequency { get; }

            public IDictionary<Term, double> TermFrequencyRatio =>
                TermFrequency.ToDictionary(f => f.Key, f => f.Value / TermCount);
            public float TermCount { get; private set; } = 0;
            
            public Document(string key)
            {
                Key = key;
                TermFrequency = new Dictionary<Term, double>();
            }

            public bool HasTerm(Term term)
            {
                return TermFrequency.ContainsKey(term);
            }
            
            public void Add(string word, PartOfSpeech? pos)
            {
                TermCount++;
                var w = CreateTerm(word.Trim().ToLower(), pos: pos);
                if (!TermFrequency.TryGetValue(w, out var count))
                {
                    TermFrequency.Add(w, 1);
                }
                else
                {
                    TermFrequency[w] = count + 1;
                }
            }
        }

        public class Term : IEquatable<Term>
        {
            public string Text { get; }
            
            public HashSet<string> Synonyms { get; }
            
            public double Score { get; }
            
            public PartOfSpeech? PoS { get; }
            
            public Term(string text, double score, PartOfSpeech? pos, IEnumerable<string> synonyms)
            {
                Text = text;
                PoS = pos;
                Score = score;
                Synonyms = new HashSet<string>(synonyms,StringComparer.InvariantCultureIgnoreCase);
            }

            public Term SetScore(double score)
            {
                return new Term(Text, score, PoS, Synonyms);
            }

            public bool IsSynonym(string word)
            {
                return String.Equals(Text, word, StringComparison.InvariantCultureIgnoreCase) 
                       ||
                       Synonyms.Contains(word);
            }
            
            public bool IsEquivalent(Term term)
            {
                return String.Equals(Text, term.Text, StringComparison.InvariantCultureIgnoreCase)
                       || Synonyms.Contains(term.Text)
                       || Synonyms.Overlaps(term.Synonyms);
            }


            public bool Equals(Term other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return string.Equals(Text, other.Text);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((Term) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((Text != null ? Text.GetHashCode() : 0) * 397);
                }
            }
        }
        
        public class Vocabulary
        {
            public double? PercentageCutOff { get; }
            
            [JsonIgnore]
            public Term[] Terms { get; }
            
            public Term[] TopTerms { get; }

            public Vocabulary(IEnumerable<Term> vocab, double percentageCutOff = 20)
            {
                PercentageCutOff = percentageCutOff;
                var termArray = vocab as Term[] ?? vocab.ToArray();
                Terms = termArray.ToArray();
                var max = Terms.Max(t => t.Score);
                var min = Terms.Min(t => t.Score);
                var percent = (max - min) / 100.0;
                var threshold = (percentageCutOff * percent) + min;
                TopTerms = termArray.Where(p => p.Score >= threshold).OrderByDescending(r => r.Score).ToArray();
            }

            public bool IsMatch(Vocabulary other)
            {
                return TopTerms.Any(t => other.TopTerms.Any(t1 => t1.IsEquivalent(t)));
            }
        }

        public class TfIdf
        {

            public Document[] Documents { get; }

            public IDictionary<string, IEnumerable<Term>> Values { get; } = new Dictionary<string, IEnumerable<Term>>();

            public IDictionary<Term, double> TermFrequencies { get; }
            
            public IDictionary<Term, int> TermDocumentCount { get; }

            
            public TfIdf(IEnumerable<Document> documents)
            {
                Documents = documents.ToArray();

                var groupedTerms =
                    Documents.SelectMany(d => d.TermFrequencyRatio)
                        .GroupBy(s => s.Key).ToArray();
                
                TermFrequencies = 
                    groupedTerms.ToDictionary(s => s.Key, s => s.Sum(x => x.Value));

                TermDocumentCount =
                    groupedTerms.ToDictionary(s => s.Key, s => s.Count());
                
                foreach (var document in Documents)
                {
                    var tfidf = new List<Term>();
                    foreach (var termFrequency in document.TermFrequencyRatio)
                    {
                        var docsWithTerm = TermDocumentCount[termFrequency.Key];
                        var term = termFrequency.Key.SetScore(
                            termFrequency.Value * Math.Log10(Documents.Length / 1 + docsWithTerm));
                        
                        tfidf.Add(term);
                        
                    }
                    
                    Values.Add(document.Key, tfidf);
                }
            }
        }

        private static StanfordCoreNLP Pipeline { get; set; }
        private static HashSet<string> Stopwords { get; set; }
        private static WordNetEngine WordNetEngine { get; set; }
        private static string Model { get; set; }
        private static string ModelDir { get; set; }

        static NLP()
        {
            var workingDirectory = Directory.GetCurrentDirectory();
            WordNetEngine = new WordNetEngine();
            WordNetEngine.LoadFromDirectory(workingDirectory + @"\wordnet");
            ModelDir = workingDirectory + @"\corenlp\models";
            Model = ModelDir + @"\edu\stanford\nlp\models\lexparser\englishPCFG.ser.gz";
            Pipeline = MakePipeline(ModelDir);
            Stopwords = new HashSet<string>(File.ReadLines(Path.Combine(workingDirectory, "stopwords.txt")),StringComparer.InvariantCultureIgnoreCase);
        }
        

        private static StanfordCoreNLP MakePipeline(string modelDir)
        {
            var workingDir = Directory.GetCurrentDirectory();
            var props = new Properties();
            props.setProperty("annotators", "tokenize,cleanxml,ssplit,pos,lemma");
            props.setProperty("clean.singlesentencetags", "LI");
            props.setProperty("clean.sentenceendingtags", "P");
            props.setProperty("ner.useSUTime", "0");
            
            Directory.SetCurrentDirectory(modelDir);
            
            var result = new StanfordCoreNLP(props);

            Directory.SetCurrentDirectory(workingDir);
            return result; 
        }

        private static Document ProcessText(string key, string text, PartOfSpeech? partOfSpeech = PartOfSpeech.None)
        {
            PartOfSpeech? toPartOfSpeech(string tag)
            {
                if (tag.Length < 2)
                    return partOfSpeech;
                
                switch (tag.Substring(0,2).ToUpper())
                {
                    case "JJ": return PartOfSpeech.Adjective;
                    case "NN": return PartOfSpeech.Noun;
                    case "RB": return PartOfSpeech.Adverb;
                    case "VB" : return PartOfSpeech.Verb;
                    default: return partOfSpeech;
                }
            }
            
            var doc = new CoreDocument(text);
            Pipeline.annotate(doc);

            var tokens = 
                doc.sentences().toArray().SelectMany(s => ((CoreSentence) s).tokens().toArray());
            
            
            var result = new Document(key);

            foreach (var token in tokens.Cast<CoreLabel>())
            {
                var word = token.word();
                var pos = partOfSpeech == null ? null : toPartOfSpeech(token.tag());
                if (!Stopwords.Contains(word))
                {
                    result.Add(word, pos);
                }
            }

            return result;
        }

        public static IDictionary<string,Vocabulary> Analyse(PartOfSpeech? partOfSpeech = PartOfSpeech.None, double topTermPercentage = 20, params (string,string)[] texts)
        {
            var tfidf = new TfIdf(texts.Select(t => ProcessText(t.Item1, t.Item2, partOfSpeech)));
            var result = new Dictionary<string, Vocabulary>();
            foreach (var x in tfidf.Values)
            {
                result.Add(x.Key, new Vocabulary(x.Value, topTermPercentage));
            }

            return result;
        }

        private static string[] BuildSynonyms(string word, PartOfSpeech? pos = null)
        {
            String[] syns;
            if (pos == null || pos == PartOfSpeech.None)
            {
                syns = WordNetEngine.GetSynSets(word).SelectMany(s => s.Words).ToArray();
            }
            else
            {
                syns = WordNetEngine.GetMostCommonSynSet(word, pos.Value)?.Words?.ToArray() ?? new string[] { };
            }

            return syns.Select(s => s.Replace("_", " ").Replace("(a)", "")).Where(s => s != word).Distinct().ToArray();

        }
        
        public static Vocabulary CreateVocabulary(IDictionary<string, double> terms, PartOfSpeech? pos = null, double percentageCutOff = 20)
        {
            return new Vocabulary(terms.Select(t => CreateTerm(t.Key, t.Value ,pos)), percentageCutOff);
        }
        
        public static Vocabulary CreateVocabulary(IDictionary<string, double> terms, double percentageCutOff = 20)
        {
            return new Vocabulary(terms.Select(t => CreateTerm(t.Key, t.Value)), percentageCutOff);
        }
        
        public static Vocabulary CreateVocabulary(IEnumerable<string> terms, double percentageCutOff = 20)
        {
            return new Vocabulary(terms.Select(t => CreateTerm(t)), percentageCutOff);
        }

        public static Term CreateTerm(string word, double score = 1, PartOfSpeech? pos = null)
        {
            return new Term(word, score, pos, BuildSynonyms(word, pos));
        }
        
    }
}