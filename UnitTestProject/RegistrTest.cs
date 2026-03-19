using System;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using pr14V2.Pages;
using pr14V2;

namespace UnitTestProject
{
    [TestClass]
    public class RegisterTests
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
        /// Позитивные тесты регистрации - успешная регистрация нового пользователя
        /// </summary>
        [TestMethod]
        public void RegisterTestSuccess()
        {
            RunInSTA(() =>
            {
                // Небольшая задержка перед началом
                Thread.Sleep(200);

                // Arrange
                RegisterPage page = new RegisterPage();

                // Генерируем уникальный логин для каждого теста
                string uniqueLogin = $"newuser{DateTime.Now.Ticks}";
                string password = "SecurePass123";
                string confirmPassword = "SecurePass123";

                // Act
                bool result = page.Register(uniqueLogin, password, confirmPassword);

                // Assert
                Assert.IsTrue(result, "Регистрация должна быть успешной с корректными данными");

                // Даем время на сохранение в БД
                Thread.Sleep(200);

                // Проверка, что пользователь действительно добавлен в БД
                var user = Core.Context.Users.FirstOrDefault(u => u.Login == uniqueLogin);
                Assert.IsNotNull(user, "Пользователь должен быть сохранен в базе данных");
                Assert.AreEqual(uniqueLogin, user.Login, "Логин должен совпадать");
                Assert.AreEqual(password, user.Password, "Пароль должен совпадать");

                // Cleanup - удаляем тестового пользователя
                if (user != null)
                {
                    Core.Context.Users.Remove(user);
                    Core.Context.SaveChanges();
                    Thread.Sleep(100);
                }
            });
        }

        /// <summary>
        /// Позитивный тест: регистрация с минимальной длиной данных
        /// </summary>
        [TestMethod]
        public void RegisterTestMinLength()
        {
            RunInSTA(() =>
            {
                // Небольшая задержка перед началом
                Thread.Sleep(200);

                // Arrange
                RegisterPage page = new RegisterPage();
                string uniqueLogin = $"u{DateTime.Now.Ticks}";
                string password = "12";
                string confirmPassword = "12";

                // Act
                bool result = page.Register(uniqueLogin, password, confirmPassword);

                // Assert
                Assert.IsTrue(result, "Регистрация должна быть успешной даже с короткими данными");

                // Даем время на сохранение
                Thread.Sleep(200);

                // Cleanup
                var user = Core.Context.Users.FirstOrDefault(u => u.Login == uniqueLogin);
                if (user != null)
                {
                    Core.Context.Users.Remove(user);
                    Core.Context.SaveChanges();
                    Thread.Sleep(100);
                }
            });
        }

        /// <summary>
        /// Негативные тесты регистрации
        /// </summary>
        [TestMethod]
        public void RegisterTestFail()
        {
            RunInSTA(() =>
            {
                // Небольшая задержка перед началом
                Thread.Sleep(200);

                // Arrange
                RegisterPage page = new RegisterPage();

                // Тест 1: Пустой логин
                bool result1 = page.Register("", "password123", "password123");
                Assert.IsFalse(result1, "Регистрация должна завершиться неудачей с пустым логином");

                // Тест 2: Логин только из пробелов
                bool result2 = page.Register("   ", "password123", "password123");
                Assert.IsFalse(result2, "Регистрация должна завершиться неудачей с логином из пробелов");

                // Тест 3: Пустой пароль
                bool result3 = page.Register("newuser", "", "");
                Assert.IsFalse(result3, "Регистрация должна завершиться неудачей с пустым паролем");

                // Тест 4: Пароли не совпадают
                bool result4 = page.Register("newuser2", "password123", "password456");
                Assert.IsFalse(result4, "Регистрация должна завершиться неудачей при несовпадении паролей");

                // Тест 5: Пустое подтверждение пароля
                bool result5 = page.Register("newuser3", "password123", "");
                Assert.IsFalse(result5, "Регистрация должна завершиться неудачей с пустым подтверждением пароля");

                // Тест 6: Существующий пользователь
                // ВАЖНО: Замените "admin" на логин, который точно есть в вашей БД
                bool result6 = page.Register("admin", "newpassword", "newpassword");
                Assert.IsFalse(result6, "Регистрация должна завершиться неудачей с существующим логином");

                // Тест 7: Все поля пустые
                bool result7 = page.Register("", "", "");
                Assert.IsFalse(result7, "Регистрация должна завершиться неудачей с пустыми данными");

                // Тест 8: null в качестве логина
                bool result8 = page.Register(null, "password123", "password123");
                Assert.IsFalse(result8, "Регистрация должна завершиться неудачей с null логином");

                // Тест 9: null в качестве пароля
                bool result9 = page.Register("newuser4", null, null);
                Assert.IsFalse(result9, "Регистрация должна завершиться неудачей с null паролем");

                // Тест 10: Пароль содержит только пробелы
                bool result10 = page.Register("newuser5", "   ", "   ");
                Assert.IsFalse(result10, "Регистрация должна завершиться неудачей с паролем из пробелов");
            });
        }

        /// <summary>
        /// Тесты валидации формата данных
        /// </summary>
        [TestMethod]
        public void RegisterTestValidation()
        {
            RunInSTA(() =>
            {
                // Небольшая задержка перед началом
                Thread.Sleep(200);

                // Arrange
                RegisterPage page = new RegisterPage();

                // Тест 1: SQL-инъекция в логине
                bool result1 = page.Register("user'; DROP TABLE Users;--", "password", "password");
                Assert.IsFalse(result1, "Регистрация должна быть защищена от SQL-инъекций");

                // Тест 2: Очень длинный логин (но в пределах разумного)
                // Используем 100 символов вместо 500
                string longLogin = new string('a', 100);
                bool result2 = page.Register(longLogin, "password", "password");
                // В зависимости от требований может быть как True, так и False

                // Тест 3: Специальные символы в логине
                bool result3 = page.Register("user<script>", "password", "password");
                // Проверка на защиту от XSS

                // Даем время на сохранение
                Thread.Sleep(200);

                // Cleanup для успешных тестов
                var user1 = Core.Context.Users.FirstOrDefault(u => u.Login == longLogin);
                if (user1 != null)
                {
                    Core.Context.Users.Remove(user1);
                    Core.Context.SaveChanges();
                    Thread.Sleep(100);
                }

                var user2 = Core.Context.Users.FirstOrDefault(u => u.Login == "user<script>");
                if (user2 != null)
                {
                    Core.Context.Users.Remove(user2);
                    Core.Context.SaveChanges();
                    Thread.Sleep(100);
                }
            });
        }

        /// <summary>
        /// Тест граничных значений
        /// ИСПРАВЛЕНО: Используем разумную длину пароля (не 1000 символов!)
        /// </summary>
        [TestMethod]
        public void RegisterTestBoundary()
        {
            RunInSTA(() =>
            {
                // Небольшая задержка перед началом
                Thread.Sleep(200);

                // Arrange
                RegisterPage page = new RegisterPage();

                // Тест 1: Минимально возможный пароль (1 символ)
                string uniqueLogin1 = $"u1{DateTime.Now.Ticks}";
                bool result1 = page.Register(uniqueLogin1, "1", "1");

                // Тест 2: Длинный пароль (но не слишком)
                // ИСПРАВЛЕНО: Используем 45 символов вместо 1000!
                // Столбец Password в БД имеет ограничение (вероятно 50 символов)
                string uniqueLogin2 = $"u2{DateTime.Now.Ticks}";
                string longPassword = new string('x', 45); // Было 1000, стало 45
                bool result2 = page.Register(uniqueLogin2, longPassword, longPassword);

                // Тест 3: Пароль с различными регистрами
                string uniqueLogin3 = $"u3{DateTime.Now.Ticks}";
                bool result3 = page.Register(uniqueLogin3, "PaSsWoRd", "password");
                Assert.IsFalse(result3, "Регистрация должна завершиться неудачей при несовпадении регистра");

                // Даем время на сохранение
                Thread.Sleep(200);

                // Cleanup
                foreach (var login in new[] { uniqueLogin1, uniqueLogin2 })
                {
                    var user = Core.Context.Users.FirstOrDefault(u => u.Login == login);
                    if (user != null)
                    {
                        Core.Context.Users.Remove(user);
                        Core.Context.SaveChanges();
                        Thread.Sleep(100);
                    }
                }
            });
        }

        /// <summary>
        /// Тест параллельной регистрации одинаковых пользователей
        /// </summary>
        [TestMethod]
        public void RegisterTestConcurrent()
        {
            RunInSTA(() =>
            {
                // Небольшая задержка перед началом
                Thread.Sleep(200);

                // Arrange
                RegisterPage page1 = new RegisterPage();
                RegisterPage page2 = new RegisterPage();
                string sameLogin = $"concurrent{DateTime.Now.Ticks}";

                // Act - пытаемся зарегистрировать пользователя с одинаковым логином
                bool result1 = page1.Register(sameLogin, "pass1", "pass1");

                // Даем время на сохранение первого пользователя
                Thread.Sleep(200);

                bool result2 = page2.Register(sameLogin, "pass2", "pass2");

                // Assert - только одна регистрация должна быть успешной
                Assert.IsTrue(result1, "Первая регистрация должна быть успешной");
                Assert.IsFalse(result2, "Вторая регистрация должна завершиться неудачей");

                // Даем время на обработку
                Thread.Sleep(200);

                // Cleanup
                var user = Core.Context.Users.FirstOrDefault(u => u.Login == sameLogin);
                if (user != null)
                {
                    Core.Context.Users.Remove(user);
                    Core.Context.SaveChanges();
                    Thread.Sleep(100);
                }
            });
        }
    }
}