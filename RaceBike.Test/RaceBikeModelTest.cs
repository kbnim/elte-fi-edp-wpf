using Moq;
using RaceBike.Model.Classes;
using RaceBike.Model;
using RaceBike.Persistence;

namespace RaceBike.Test
{
    [TestClass]
    public class RaceBikeModelTest
    {
        private RaceBikeModel _model = null!;
        private RaceBikeFile _file;
        private Mock<IRaceBikeDataAccess> _mock = null!;

        [TestInitialize]
        public void Initialize()
        {
            _mock = new Mock<IRaceBikeDataAccess>();

            _model = new RaceBikeModel(_mock.Object);
            _model.GameContinues += Model_GameContinues;
            _model.GameOver += Model_GameOver;
        }

        private async Task LoadFileContents(string contents)
        {
            List<string> lines = contents.Split(Environment.NewLine, StringSplitOptions.TrimEntries).ToList();
            _file =  RaceBikeFile.Parse(lines);
            _mock.Setup(mock => mock.LoadAsync(It.IsAny<string>()))
                 .Returns(() => Task.FromResult(_file));
            await _model.LoadGameAsync(string.Empty);
        }

        [TestMethod]
        public async Task ValidInputFileTest01()
        {
            string content = "00:00:13.2795311" + Environment.NewLine +
                             "Medium" + Environment.NewLine + 
                             "Bike (202,364)" + Environment.NewLine + 
                             "Fuel (1,1)" + Environment.NewLine +
                             "Fuel (6,10)";

            await LoadFileContents(content);
            Assert.AreEqual(_file.LatestBestTime, _model.LatestBestTime);
            Assert.AreEqual((int)_file.Speed, (int)_model.CurrentSpeed);
            Assert.AreEqual(_file.Entities.First().Row, _model.BikeLocation.X);
            Assert.AreEqual(_file.Entities.First().Column, _model.BikeLocation.Y);
            Assert.AreEqual(_file.Entities.Count, _model.FuelLocations.Count() + 1);
        }

        [TestMethod]
        public async Task ValidInputFileTest02()
        {
            string content = "00:00:13.2795311" + Environment.NewLine +
                             "Medium" + Environment.NewLine +
                             "Bike (202,364)";

            await LoadFileContents(content);
            Assert.AreEqual(_file.LatestBestTime, _model.LatestBestTime);
            Assert.AreEqual((int)_file.Speed, (int)_model.CurrentSpeed);
            Assert.AreEqual(_file.Entities.First().Row, _model.BikeLocation.X);
            Assert.AreEqual(_file.Entities.First().Column, _model.BikeLocation.Y);
            Assert.AreEqual(_file.Entities.Count, _model.FuelLocations.Count() + 1);
        }

        [TestMethod]
        public async Task ValidInputFileTest03()
        {
            string content = "00:00:13.2795311" + Environment.NewLine +
                             "Medium";

            await LoadFileContents(content);
            Assert.AreEqual(_file.LatestBestTime, _model.LatestBestTime);
            Assert.AreEqual((int)_file.Speed, (int)_model.CurrentSpeed);
            Assert.AreEqual(0, _file.Entities.Count);
            Assert.AreEqual(0, _model.FuelLocations.Count());
            Assert.AreNotEqual(_file.Entities.Count, _model.FuelLocations.Count() + 1);
        }

        [TestMethod]
        [ExpectedException(typeof(RaceBikeDataException))]
        public async Task InvalidInputFileTest01()
        {
            string content = "00:00:13.2795311û" + Environment.NewLine +
                             "Medium" + Environment.NewLine +
                             "Bike (202,364)";

            await LoadFileContents(content);
        }

        [TestMethod]
        [ExpectedException(typeof(RaceBikeDataException))]
        public async Task InvalidInputFileTest02()
        {
            string content = "00:00:13.2795311" + Environment.NewLine +
                             "Mediumû" + Environment.NewLine +
                             "Bike (202,364)";

            await LoadFileContents(content);
        }

        [TestMethod]
        [ExpectedException(typeof(RaceBikeDataException))]
        public async Task InvalidInputFileTest03()
        {
            string content = "00:00:13.2795311" + Environment.NewLine +
                             "Medium" + Environment.NewLine +
                             "Bike(202,364)";

            await LoadFileContents(content);
        }

        [TestMethod]
        [ExpectedException(typeof(RaceBikeDataException))]
        public async Task InvalidInputFileTest04()
        {
            string content = "Medium" + Environment.NewLine +
                             "Bike (202,364)";

            await LoadFileContents(content);
        }

        [TestMethod]
        [ExpectedException(typeof(RaceBikeDataException))]
        public async Task InvalidInputFileTest05()
        {
            string content = "00:00:13.2795311" + Environment.NewLine +
                             "Bike (1,1)";

            await LoadFileContents(content);
        }

        [TestMethod]
        public void GamePauseResumeTest01()
        {
            _model.GameTimeResume(); // start
            _model.GameTimePause(); // stop
            Assert.AreNotEqual(TimeSpan.Zero, _model.CurrentTime);
            Assert.AreNotEqual(TimeSpan.Zero, _model.LatestBestTime);
            Assert.AreEqual(_model.LatestBestTime, _model.CurrentTime);
            Assert.AreEqual(1, (int)_model.CurrentSpeed);
        }

        [TestMethod]
        public async Task GamePauseResumeTest02()
        {
            string content = "00:00:13.2795311" + Environment.NewLine +
                             "Medium" + Environment.NewLine +
                             "Bike (202,364)" + Environment.NewLine +
                             "Fuel (1,1)" + Environment.NewLine +
                             "Fuel (6,10)";

            await LoadFileContents(content);
            _model.GameTimeResume(); // start
            _model.GameTimePause(); // stop
            Assert.AreNotEqual(TimeSpan.Zero, _model.CurrentTime);
            Assert.AreNotEqual(TimeSpan.Zero, _model.LatestBestTime);
            Assert.AreEqual(_model.LatestBestTime, _model.CurrentTime);
            Assert.AreEqual((int)_file.Speed, (int)_model.CurrentSpeed);
        }

        [TestMethod]
        public void ChangeSpeedTest()
        {
            Assert.AreEqual(1, (int)_model.CurrentSpeed);
            Assert.AreEqual("Slow", _model.CurrentSpeed.ToString());
            _model.SpeedUp();
            Assert.AreEqual(2, (int)_model.CurrentSpeed);
            Assert.AreEqual("Medium", _model.CurrentSpeed.ToString());
            _model.SpeedUp();
            Assert.AreEqual(3, (int)_model.CurrentSpeed);
            Assert.AreEqual("Fast", _model.CurrentSpeed.ToString());
            _model.SpeedUp();
            Assert.AreEqual(3, (int)_model.CurrentSpeed);
            Assert.AreEqual("Fast", _model.CurrentSpeed.ToString());
            _model.SlowDown();
            Assert.AreEqual(2, (int)_model.CurrentSpeed);
            Assert.AreEqual("Medium", _model.CurrentSpeed.ToString());
            _model.SlowDown();
            Assert.AreEqual(1, (int)_model.CurrentSpeed);
            Assert.AreEqual("Slow", _model.CurrentSpeed.ToString());
            _model.SlowDown();
            Assert.AreEqual(1, (int)_model.CurrentSpeed);
            Assert.AreEqual("Slow", _model.CurrentSpeed.ToString());
        }

        [TestMethod]
        public void RunningOutOfGasTest()
        {
            Assert.IsTrue(_model.IsPaused);
            _model.GameTimeResume(); // resumes, i.e. starts the game

            while (!_model.IsGameOver)
            {
                // Assert.IsFalse(_model.IsGameOver);
                _model.GameTimeElapsing();
            }

            Assert.AreEqual(0, _model.CurrentTankLevel);
            Assert.IsTrue(_model.IsGameOver);
            Assert.IsFalse(_model.IsPaused);
        }

        [TestMethod]
        public void FuelConsumption()
        {
            Assert.IsTrue(_model.IsPaused);
            _model.GameTimeResume(); // start

            for (int i = 0; i < 100; i++)
            {
                _model.GameTimeElapsing();
            }

            _model.GenerateNewFuelItem();
            _model.IncreaseTankLevel();

            Assert.AreEqual(100, _model.CurrentTankLevel);
            Assert.IsFalse(_model.IsGameOver);
            Assert.IsFalse(_model.IsPaused);
        }

        [TestMethod]
        public void FuelConsumptionEmptyQueue()
        {
            for (int i = 0; i < 100; i++) // (1000 - 100) ÷ 10 = 90
            {
                _model.GameTimeElapsing();
            }

            Assert.AreEqual(90, _model.CurrentTankLevel);
            _model.IncreaseTankLevel(); // no fuel items have been generated
            Assert.AreEqual(90, _model.CurrentTankLevel);
        }

        [TestMethod]
        public void TestReset()
        {
            _model.GameTimeResume(); // start

            for (int i = 0; i < 100; i++)
            {
                _model.SpeedUp();
                _model.GameTimeElapsing();
            }

            TimeSpan current = _model.CurrentTime;
            ImmutableSpeed speed = _model.CurrentSpeed;
            int level = _model.CurrentTankLevel;
            _model.Reset();
            Assert.AreNotEqual(current, _model.CurrentTime);
            Assert.AreNotEqual(speed, _model.CurrentSpeed);
            Assert.AreNotEqual(level, _model.CurrentTankLevel);
        }

        private void Model_GameOver(object? sender, EventArgs e)
        {
            Assert.IsTrue(_model.IsPaused);
        }

        private void Model_GameContinues(object? sender, EventArgs e)
        {
            Assert.IsTrue(_model.CurrentTime >= TimeSpan.Zero);
        }
    }
}