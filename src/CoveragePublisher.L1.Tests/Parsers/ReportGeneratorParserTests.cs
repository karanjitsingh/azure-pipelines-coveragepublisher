using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Azure.Pipelines.CoveragePublisher.Parsers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace CoveragePublisher.L1.Tests
{
    [TestClass]
    public class ReportGeneratorParserTests
    {
        [TestMethod]
        [DataRow(new string[] { "SampleCoverage/Clover.xml" }, "[{\"LineCoverageStatus\":{\"1\":0,\"4\":0},\"FilePath\":\"$PROJECT_PATH/src/errors.ts\"},{\"LineCoverageStatus\":{\"1\":0,\"3\":0,\"4\":0,\"6\":0,\"7\":0,\"9\":0,\"10\":0,\"12\":0,\"13\":0},\"FilePath\":\"$PROJECT_PATH/src/test.ts\"}]")]
        [DataRow(new string[] { "SampleCoverage/Cobertura.xml" }, "[{\"LineCoverageStatus\":{\"3\":0,\"4\":0,\"5\":0},\"FilePath\":\"C:\\\\temp\\\\test\\\\AbstractClass.java\"},{\"LineCoverageStatus\":{\"3\":0,\"4\":0,\"5\":0,\"8\":1,\"12\":1},\"FilePath\":\"C:\\\\temp\\\\test\\\\AbstractClass_SampleImpl1.java\"},{\"LineCoverageStatus\":{\"3\":0,\"4\":0,\"5\":0,\"8\":1,\"12\":1},\"FilePath\":\"C:\\\\temp\\\\test\\\\AbstractClass_SampleImpl2.java\"},{\"LineCoverageStatus\":{\"2\":0,\"4\":0,\"5\":0},\"FilePath\":\"C:\\\\temp\\\\test\\\\GenericClass.java\"},{\"LineCoverageStatus\":{\"2\":1,\"5\":0,\"7\":0,\"9\":0,\"10\":0,\"11\":0},\"FilePath\":\"C:\\\\temp\\\\test\\\\Program.java\"},{\"LineCoverageStatus\":{\"4\":0,\"7\":0,\"12\":0,\"13\":0,\"15\":0,\"17\":0,\"20\":1,\"26\":0,\"28\":1,\"30\":1,\"34\":1},\"FilePath\":\"C:\\\\temp\\\\test\\\\TestClass.java\"},{\"LineCoverageStatus\":{\"3\":1,\"5\":1,\"9\":1},\"FilePath\":\"C:\\\\temp\\\\test\\\\sub\\\\Sub.java\"}]")]
        [DataRow(new string[] { "SampleCoverage/Jacoco.xml" }, "[{\"LineCoverageStatus\":{\"3\":0,\"4\":0,\"5\":0},\"FilePath\":\"AbstractClass.java\"},{\"LineCoverageStatus\":{\"3\":0,\"4\":0,\"5\":0,\"8\":1,\"12\":1},\"FilePath\":\"AbstractClass_SampleImpl1.java\"},{\"LineCoverageStatus\":{\"3\":0,\"4\":0,\"5\":0,\"8\":1,\"12\":1},\"FilePath\":\"AbstractClass_SampleImpl2.java\"},{\"LineCoverageStatus\":{\"2\":0,\"4\":0,\"5\":0},\"FilePath\":\"GenericClass.java\"},{\"LineCoverageStatus\":{\"2\":1,\"5\":0,\"7\":0,\"9\":0,\"10\":0,\"11\":0},\"FilePath\":\"Program.java\"},{\"LineCoverageStatus\":{\"4\":0,\"7\":0,\"12\":0,\"13\":0,\"15\":0,\"17\":0,\"20\":1,\"24\":0,\"25\":0,\"26\":0,\"28\":1,\"30\":1,\"34\":1},\"FilePath\":\"TestClass.java\"},{\"LineCoverageStatus\":{\"3\":1,\"5\":1,\"9\":1},\"FilePath\":\"Sub.java\"}]")]
        [DataRow(new string[] { "SampleCoverage/Clover.xml", "SampleCoverage/Cobertura.xml", "SampleCoverage/Jacoco.xml" }, "[{\"LineCoverageStatus\":{\"1\":0,\"4\":0},\"FilePath\":\"$PROJECT_PATH/src/errors.ts\"},{\"LineCoverageStatus\":{\"1\":0,\"3\":0,\"4\":0,\"6\":0,\"7\":0,\"9\":0,\"10\":0,\"12\":0,\"13\":0},\"FilePath\":\"$PROJECT_PATH/src/test.ts\"},{\"LineCoverageStatus\":{\"3\":0,\"4\":0,\"5\":0},\"FilePath\":\"C:\\\\temp\\\\test\\\\AbstractClass.java\"},{\"LineCoverageStatus\":{\"3\":0,\"4\":0,\"5\":0,\"8\":1,\"12\":1},\"FilePath\":\"C:\\\\temp\\\\test\\\\AbstractClass_SampleImpl1.java\"},{\"LineCoverageStatus\":{\"3\":0,\"4\":0,\"5\":0,\"8\":1,\"12\":1},\"FilePath\":\"C:\\\\temp\\\\test\\\\AbstractClass_SampleImpl2.java\"},{\"LineCoverageStatus\":{\"2\":0,\"4\":0,\"5\":0},\"FilePath\":\"C:\\\\temp\\\\test\\\\GenericClass.java\"},{\"LineCoverageStatus\":{\"2\":1,\"5\":0,\"7\":0,\"9\":0,\"10\":0,\"11\":0},\"FilePath\":\"C:\\\\temp\\\\test\\\\Program.java\"},{\"LineCoverageStatus\":{\"4\":0,\"7\":0,\"12\":0,\"13\":0,\"15\":0,\"17\":0,\"20\":1,\"26\":0,\"28\":1,\"30\":1,\"34\":1},\"FilePath\":\"C:\\\\temp\\\\test\\\\TestClass.java\"},{\"LineCoverageStatus\":{\"3\":0,\"4\":0,\"5\":0},\"FilePath\":\"AbstractClass.java\"},{\"LineCoverageStatus\":{\"3\":0,\"4\":0,\"5\":0,\"8\":1,\"12\":1},\"FilePath\":\"AbstractClass_SampleImpl1.java\"},{\"LineCoverageStatus\":{\"3\":0,\"4\":0,\"5\":0,\"8\":1,\"12\":1},\"FilePath\":\"AbstractClass_SampleImpl2.java\"},{\"LineCoverageStatus\":{\"2\":0,\"4\":0,\"5\":0},\"FilePath\":\"GenericClass.java\"},{\"LineCoverageStatus\":{\"2\":1,\"5\":0,\"7\":0,\"9\":0,\"10\":0,\"11\":0},\"FilePath\":\"Program.java\"},{\"LineCoverageStatus\":{\"4\":0,\"7\":0,\"12\":0,\"13\":0,\"15\":0,\"17\":0,\"20\":1,\"24\":0,\"25\":0,\"26\":0,\"28\":1,\"30\":1,\"34\":1},\"FilePath\":\"TestClass.java\"},{\"LineCoverageStatus\":{\"3\":1,\"5\":1,\"9\":1},\"FilePath\":\"C:\\\\temp\\\\test\\\\sub\\\\Sub.java\"},{\"LineCoverageStatus\":{\"3\":1,\"5\":1,\"9\":1},\"FilePath\":\"Sub.java\"}]")]
        public void WillGenerateCorrectFileCoverage(string[] coverageFiles, string result)
        {
            var parser = new ReportGeneratorParser();
            var fileCoverages = parser.GetFileCoverageInfos(coverageFiles.ToList());
            var json = JsonConvert.SerializeObject(fileCoverages);

            Assert.AreEqual(json, result);
        }

        [TestMethod]
        [DataRow(new string[] { "SampleCoverage/Clover.xml" }, "{\"CoverageStats\":[{\"Label\":\"line\",\"Position\":4,\"Total\":11,\"Covered\":11,\"IsDeltaAvailable\":false,\"Delta\":0.0}],\"BuildPlatform\":\"\",\"BuildFlavor\":\"\"}")]
        [DataRow(new string[] { "SampleCoverage/Cobertura.xml" }, "{\"CoverageStats\":[{\"Label\":\"line\",\"Position\":4,\"Total\":36,\"Covered\":24,\"IsDeltaAvailable\":false,\"Delta\":0.0}],\"BuildPlatform\":\"\",\"BuildFlavor\":\"\"}")]
        [DataRow(new string[] { "SampleCoverage/Jacoco.xml" }, "{\"CoverageStats\":[{\"Label\":\"line\",\"Position\":4,\"Total\":38,\"Covered\":26,\"IsDeltaAvailable\":false,\"Delta\":0.0}],\"BuildPlatform\":\"\",\"BuildFlavor\":\"\"}")]
        [DataRow(new string[] { "SampleCoverage/Clover.xml", "SampleCoverage/Cobertura.xml", "SampleCoverage/Jacoco.xml" }, "{\"CoverageStats\":[{\"Label\":\"line\",\"Position\":4,\"Total\":85,\"Covered\":61,\"IsDeltaAvailable\":false,\"Delta\":0.0}],\"BuildPlatform\":\"\",\"BuildFlavor\":\"\"}")]
        public void WillGenerateCorrectCoverageSummary(string[] coverageFiles, string result)
        {
            var parser = new ReportGeneratorParser();
            var summary = parser.GetCoverageSummary(coverageFiles.ToList());
            var json = JsonConvert.SerializeObject(summary.CodeCoverageData);

            Assert.AreEqual(json, result);
        }

        [TestMethod]
        public void WillReturnEmptyCoverageForNoInputFiles()
        {
            var parser = new ReportGeneratorParser();
            var fileCoverage = parser.GetFileCoverageInfos(new List<string>());
            var summary = parser.GetCoverageSummary(new List<string>());

            Assert.AreEqual(fileCoverage.Count, 0);
            Assert.AreEqual(summary.CodeCoverageData.CoverageStats[0].Total, 0);
        }

        [TestMethod]
        public void WillReturnEmptyCoverageForNonExistingFile()
        {
            var parser = new ReportGeneratorParser();
            var fileCoverage = parser.GetFileCoverageInfos(new List<string>() { "SampleCoverage/blabla.xml" });
            var summary = parser.GetCoverageSummary(new List<string>() { "SampleCoverage/blabla.xml" });

            Assert.AreEqual(fileCoverage.Count, 0);
            Assert.AreEqual(summary.CodeCoverageData.CoverageStats[0].Total, 0);
        }
    }
}
