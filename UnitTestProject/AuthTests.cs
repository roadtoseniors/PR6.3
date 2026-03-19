using System;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using pr14V2.Pages;
using pr14V2;

namespace UnitTestProject
{
    [TestClass]
    public class AuthTests
    {
        /// <summary>
        /// Очистка после каждого теста
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            // Даем время на освобождение ресурсов
            Thread.Sleep(100);

            // Пробуем освободить контекст, если есть метод Dispose
            try
            {
                if (Core.Context != null)
                {
                    // Если Context имеет метод Dispose
                    var disposable = Core.Context as IDisposable;
                    disposable?.Dispose();
                }
            }
            catch
            {
                // Игнорируем ошибки при очистке
            }

            // Еще немного подождем
            Thread.Sleep(100);
        }

        /// <summary>
        /// Вспомогательный метод для выполнения теста в STA потоке
        /// </summary>
        private void RunInSTA(Action test)
        {
            Exception exception = null;
            var thread = new Thread(() =>
            {
                try
                {
                    test();
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();

            if (exception != null)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Тест авторизации с неверными данными
        /// </summary>
        [TestMethod]
        public void AuthTest()
        {
            RunInSTA(() =>
            {
                // Небольшая задержка перед началом
                Thread.Sleep(200);

                // Arrange - подготовка данных
                LoginPage page = new LoginPage();
                string login = "wronguser";
                string password = "wrongpassword";

                // Act - выполнение действия
                bool result = page.Auth(login, password);

                // Assert - проверка результата
                Assert.IsFalse(result, "Авторизация должна завершиться неудачей с неверными данными");
            });
        }

        /// <summary>
        /// Тест успешной авторизации для всех пользователей в БД
        /// ВАЖНО: Замените данные на реальные из вашей БД!
        /// </summary>
        [TestMethod]
        public void AuthTestSuccess()
        {
            RunInSTA(() =>
            {
                // Небольшая задержка перед началом
                Thread.Sleep(200);

                // Arrange
                LoginPage page = new LoginPage();

                // ВАЖНО: замените эти данные на реальные логины и пароли из вашей БД!
                var testUsers = new[]
                {
                    new { Login = "admin", Password = "admin123" },
                    new { Login = "123", Password = "123" },
                    // Добавьте своих пользователей:
                    // new { Login = "ваш_логин", Password = "ваш_пароль" },
                };

                // Act & Assert
                foreach (var testUser in testUsers)
                {
                    bool result = page.Auth(testUser.Login, testUser.Password);
                    Assert.IsTrue(result,
                        $"Авторизация должна быть успешной для пользователя {testUser.Login}");

                    // Небольшая задержка между проверками пользователей
                    Thread.Sleep(100);
                }
            });
        }

        /// <summary>
        /// Тесты негативных сценариев авторизации
        /// </summary>
        [TestMethod]
        public void AuthTestFail()
        {
            RunInSTA(() =>
            {
                // Небольшая задержка перед началом
                Thread.Sleep(200);

                // Arrange
                LoginPage page = new LoginPage();

                // Тест 1: Пустой логин
                bool result1 = page.Auth("", "password");
                Assert.IsFalse(result1, "Авторизация должна завершиться неудачей с пустым логином");

                // Тест 2: Пустой пароль
                bool result2 = page.Auth("admin", "");
                Assert.IsFalse(result2, "Авторизация должна завершиться неудачей с пустым паролем");

                // Тест 3: Оба поля пустые
                bool result3 = page.Auth("", "");
                Assert.IsFalse(result3, "Авторизация должна завершиться неудачей с пустыми данными");

                // Тест 4: Несуществующий логин
                bool result4 = page.Auth("nonexistentuser12345", "password123");
                Assert.IsFalse(result4, "Авторизация должна завершиться неудачей с несуществующим логином");

                // Тест 5: Правильный логин, неверный пароль
                bool result5 = page.Auth("admin", "wrongpassword12345");
                Assert.IsFalse(result5, "Авторизация должна завершиться неудачей с неверным паролем");

                // Тест 6: Логин с пробелами
                bool result6 = page.Auth("   ", "password");
                Assert.IsFalse(result6, "Авторизация должна завершиться неудачей с логином из пробелов");

                // Тест 7: Пароль с пробелами
                bool result7 = page.Auth("admin", "   ");
                Assert.IsFalse(result7, "Авторизация должна завершиться неудачей с паролем из пробелов");

                // Тест 8: SQL-инъекция в логине
                bool result8 = page.Auth("admin' OR '1'='1", "password");
                Assert.IsFalse(result8, "Авторизация должна завершиться неудачей при попытке SQL-инъекции");

                // Тест 9: Очень длинный логин
                string longLogin = new string('a', 1000);
                bool result9 = page.Auth(longLogin, "password");
                Assert.IsFalse(result9, "Авторизация должна завершиться неудачей с очень длинным логином");

                // Тест 10: Специальные символы в логине
                bool result10 = page.Auth("!@#$%^&*()", "password");
                Assert.IsFalse(result10, "Авторизация должна завершиться неудачей со спецсимволами в логине");
            });
        }

        /// <summary>
        /// Дополнительные тесты на граничные случаи
        /// </summary>
        [TestMethod]
        public void AuthTestBoundary()
        {
            RunInSTA(() =>
            {
                // Небольшая задержка перед началом
                Thread.Sleep(200);

                // Arrange
                LoginPage page = new LoginPage();

                // Тест: null в качестве логина
                bool result1 = page.Auth(null, "password");
                Assert.IsFalse(result1, "Авторизация должна завершиться неудачей с null логином");

                // Тест: null в качестве пароля
                bool result2 = page.Auth("admin", null);
                Assert.IsFalse(result2, "Авторизация должна завершиться неудачей с null паролем");

                // Тест: оба параметра null
                bool result3 = page.Auth(null, null);
                Assert.IsFalse(result3, "Авторизация должна завершиться неудачей с null параметрами");
            });
        }
    }
}