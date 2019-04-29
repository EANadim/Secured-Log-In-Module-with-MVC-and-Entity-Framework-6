using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace WebApplication1.Models
{
    public class Account
    {
        [Key]
        public string Email { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Type { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public int Attempt { get; set; }
        public DateTime Blocktime { get; set; }

        public bool Insert(Account ac)
        {
            using (AccountDBContext conn = new AccountDBContext())
            {
                try
                {
                    string password = ac.Password;

                    using (MD5 md5Hash = MD5.Create())
                    {
                        string hash = GetMd5Hash(md5Hash, password);
                        if (VerifyMd5Hash(md5Hash, password, hash))
                        {
                            password = hash;
                        }
                        else
                        {

                        }
                        password = hash;
                    }
                    ac.Password = password;
                    password = "";

                    ac.Attempt = 0;
                    ac.Blocktime = DateTime.Now.AddDays(-1);
                    conn.Accounts.Add(ac);
                    conn.SaveChanges();

                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }
        public bool UpdatePassword(string email,string password)
        {
            try
            {
                using (AccountDBContext db = new AccountDBContext())
                {
                    using (MD5 md5Hash = MD5.Create())
                    {
                        string hash = GetMd5Hash(md5Hash, password);
                        if (VerifyMd5Hash(md5Hash, password, hash))
                        {
                            password = hash;
                        }
                        else
                        {

                        }
                        password = hash;
                    }
                    var result = db.Accounts.SingleOrDefault(b => b.Email == email.ToString());
                    if (result != null)
                    {
                        result.Password = password;
                        db.SaveChanges();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public void UpdateAttempt(string email)
        {
            using (AccountDBContext db = new AccountDBContext())
            {
                var result = db.Accounts.SingleOrDefault(b => b.Email == email.ToString());
                if (result != null)
                {
                    result.Attempt = result.Attempt + 1;
                    db.SaveChanges();
                }
            }
        }
        public void UpdateAttemptToZero(string email)
        {
            using (AccountDBContext db = new AccountDBContext())
            {
                var result = db.Accounts.SingleOrDefault(b => b.Email == email.ToString());
                if (result != null)
                {
                    result.Attempt = 0;
                    db.SaveChanges();
                }
            }
        }
        public bool UpdateBlockTime(string email, int min)
        {
            using (AccountDBContext db = new AccountDBContext())
            {
                var result = db.Accounts.SingleOrDefault(b => b.Email == email.ToString());
                if (result != null)
                {
                    result.Blocktime = DateTime.Now.AddMinutes(min);
                    db.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        static string GetMd5Hash(MD5 md5Hash, string input)
        {

            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
        static bool VerifyMd5Hash(MD5 md5Hash, string input, string hash)
        {
            string hashOfInput = GetMd5Hash(md5Hash, input);
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;
            if (0 == comparer.Compare(hashOfInput, hash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    public class AccountDBContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
    }
}