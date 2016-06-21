namespace Gu.Localization.Tests
{
    using System;
    using System.Globalization;
    using System.Linq;

    using NUnit.Framework;

    public partial class TranslatorGenericTests
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            TestHelpers.ClearTranslationCache();
        }

        [TestCase("AllLanguages", "en", "English")]
        [TestCase("AllLanguages", "sv", "Svenska")]
        [TestCase("AllLanguages", null, "So neutral")]
        public void HappyPath(string key, string culture, string expected)
        {
            Translator.CurrentCulture = culture == null
                                            ? CultureInfo.InvariantCulture
                                            : CultureInfo.GetCultureInfo(culture);
            var actual = Translator<Properties.Resources>.Translate(nameof(Properties.Resources.AllLanguages));
            Assert.AreEqual(expected, actual);

            foreach (var errorHandling in Enum.GetValues(typeof(ErrorHandling)).OfType<ErrorHandling>())
            {
                actual = Translator<Properties.Resources>.Translate(nameof(Properties.Resources.AllLanguages), errorHandling);
                Assert.AreEqual(expected, actual);
            }

            foreach (var errorHandling in Enum.GetValues(typeof(ErrorHandling)).OfType<ErrorHandling>())
            {
                Translator.ErrorHandling = errorHandling;
                actual = Translator<Properties.Resources>.Translate(nameof(Properties.Resources.AllLanguages));
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void CreateTranslation()
        {
            Translator.CurrentCulture = CultureInfo.GetCultureInfo("en");
            var translation = Translator<Properties.Resources>.GetOrCreateTranslation(nameof(Properties.Resources.AllLanguages));

            Assert.AreEqual("English", translation.Translated);
            Translator.CurrentCulture = CultureInfo.GetCultureInfo("sv");
            Assert.AreEqual("Svenska", translation.Translated);
        }

        [TestCaseSource(typeof(TranslationErrorsSource))]
        public void WithGlobalErrorhandling(TranslationErrorsSource.ErrorData data)
        {
            if (!Translator.ContainsCulture(data.Culture))
            {
                Assert.Pass("nop");
            }

            Translator.CurrentCulture = data.Culture;
            Translator.ErrorHandling = data.ErrorHandling;
            var actual = Translator<Properties.Resources>.Translate(data.Key);
            Assert.AreEqual(data.ExpectedTranslation, actual);
        }

        [TestCaseSource(typeof(TranslationErrorsSource))]
        public void WithExplicitErrorhandling(TranslationErrorsSource.ErrorData data)
        {
            if (!Translator.ContainsCulture(data.Culture))
            {
                Assert.Pass("nop");
            }

            Translator.CurrentCulture = data.Culture;
            Translator.ErrorHandling = ErrorHandling.Throw;
            var actual = Translator<Properties.Resources>.Translate(data.Key, data.ErrorHandling);
            Assert.AreEqual(data.ExpectedTranslation, actual);
        }

        [TestCaseSource(typeof(TranslationErrorsSource))]
        public void WithExplicitErrorhandlingAndCulture(TranslationErrorsSource.ErrorData data)
        {
            Translator.CurrentCulture = null;
            Translator.ErrorHandling = ErrorHandling.Throw;
            var actual = Translator<Properties.Resources>.Translate(data.Key, data.Culture, data.ErrorHandling);
            Assert.AreEqual(data.ExpectedTranslation, actual);
        }

        [TestCaseSource(typeof(TranslationThrowSource))]
        public void ThrowsWithGlobalErrorhandling(TranslationThrowSource.ErrorData data)
        {
            if (!Translator.ContainsCulture(data.Culture))
            {
                Assert.Pass("nop");
            }

            Translator.CurrentCulture = data.Culture;
            Translator.ErrorHandling = data.ErrorHandling;
            var actual = Assert.Throws<ArgumentOutOfRangeException>(() => Translator<Properties.Resources>.Translate(data.Key));
            Assert.AreEqual(data.ExpectedMessage, actual.Message);

            actual = Assert.Throws<ArgumentOutOfRangeException>(() => Translator<Properties.Resources>.Translate(data.Key, ErrorHandling.Default));
            Assert.AreEqual(data.ExpectedMessage, actual.Message);
        }

        [TestCaseSource(typeof(TranslationThrowSource))]
        public void ThrowsWithExplicitErrorhandling(TranslationThrowSource.ErrorData data)
        {
            if (!Translator.ContainsCulture(data.Culture))
            {
                Assert.Pass("nop");
            }

            Translator.CurrentCulture = data.Culture;
            Translator.ErrorHandling = ErrorHandling.ReturnErrorInfo;
            var actual = Assert.Throws<ArgumentOutOfRangeException>(() => Translator<Properties.Resources>.Translate(data.Key, data.ErrorHandling));
            Assert.AreEqual(data.ExpectedMessage, actual.Message);
        }

        [TestCaseSource(typeof(TranslationThrowSource))]
        public void ThrowsWithExplicitErrorhandlingAndCulture(TranslationThrowSource.ErrorData data)
        {
            Translator.CurrentCulture = null;
            Translator.ErrorHandling = ErrorHandling.ReturnErrorInfo;
            var actual = Assert.Throws<ArgumentOutOfRangeException>(() => Translator<Properties.Resources>.Translate(data.Key, data.Culture, data.ErrorHandling));
            Assert.AreEqual(data.ExpectedMessage, actual.Message);
        }
    }
}