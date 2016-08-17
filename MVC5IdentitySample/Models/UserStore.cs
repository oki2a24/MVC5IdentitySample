using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using MVC5IdentitySample.Models;
using System.Linq;

namespace MVC5IdentitySample.Models
{
    internal class UserStore :
        IUserStore<User>,
        IUserStore<User, string>,
        IUserPasswordStore<User, string>
    {
        private MVC5IdentitySampleContext db;

        public UserStore(MVC5IdentitySampleContext mVC5IdentitySampleContext)
        {
            db = mVC5IdentitySampleContext;
        }

        /// <summary>
        /// ユーザーを作成します。
        /// </summary>
        /// <param name="user">ユーザーオブジェクト</param>
        /// <returns>IdentityResult オブジェクト</returns>
        public Task CreateAsync(User user)
        {
            // この時点で ID が null だと例外
            // 1 つ以上のエンティティで検証が失敗しました。詳細については 'EntityValidationErrors' プロパティを参照してください。
            // ユーザー名重複チェックが自動的になされ、OK の場合のみここに来る。
            // パスワードは自動的に暗号化済みの状態でここに来る。
            db.Users.Add(user);
            db.SaveChanges();
            return Task.FromResult(default(object));
        }

        /// <summary>
        /// ユーザーを削除します。
        /// </summary>
        /// <param name="user">ユーザーオブジェクト</param>
        /// <returns>IdentityResult オブジェクト</returns>
        public Task DeleteAsync(User user)
        {
            User deletedTargetUser = db.Users.Find(user.Id);
            db.Users.Remove(deletedTargetUser);
            db.SaveChanges();
            return Task.FromResult(default(object));
        }

        /// <summary>
        /// ユーザーを Id を指定して取得します。
        /// </summary>
        /// <param name="userId">ユーザー ID</param>
        /// <returns>ユーザーオブジェクト</returns>
        public Task<User> FindByIdAsync(string userId)
        {
            var result = db.Users.Find(userId);
            return Task.FromResult(result);
        }

        /// <summary>
        /// ユーザーをユーザー名を指定して取得します。
        /// </summary>
        /// <param name="userName">ユーザー名</param>
        /// <returns>ユーザーオブジェクト</returns>
        public Task<User> FindByNameAsync(string userName)
        {
            var result = db.Users.FirstOrDefault(x => x.UserName == userName);
            return Task.FromResult(result);
        }

        /// <summary>
        /// ユーザー情報を更新します。
        /// </summary>
        /// <param name="user">ユーザーオブジェクト</param>
        /// <returns>IdentityResult オブジェクト</returns>
        public Task UpdateAsync(User user)
        {
            // コントローラーと同じ、db.Entry(user).State = EntityState.Modified; で更新しようとすると、下記のエラー
            // 例外の詳細: System.InvalidOperationException: 同じ型の別のエンティティに同じ主キー値が既に設定されているため、
            // 型 'Identity3.Models.ApplicationUser' のエンティティをアタッチできませんでした。
            // この状況は、グラフ内のエンティティでキー値が競合している場合に 'Attach' メソッドを使用するか、
            // エンティティの状態を 'Unchanged' または 'Modified' に設定すると発生する可能性があります。
            // これは、一部のエンティティが新しく、まだデータベースによって生成されたキー値を受け取っていないことが原因である場合があります。
            // この場合は、'Add' メソッドまたは 'Added' エンティティ状態を使用してグラフを追跡してから、
            // 必要に応じて、既存のエンティティの状態を 'Unchanged' または 'Modified' に設定してください。

            var updatedTargetUser = db.Users.Find(user.Id);
            updatedTargetUser.UserName = user.UserName;
            updatedTargetUser.Memo = user.Memo;
            // Q : 特別な処理無しで、パスワードはハッシュ化されて更新されるのか？
            // A: されない。よって、UpdateAsync ではパスワードを変更しない。
            //updatedTargetUser.Password = user.Password;
            db.SaveChanges();
            return Task.FromResult(default(object));
        }

        /// <summary>
        /// ユーザーにハッシュ化されたパスワードを設定します。
        /// </summary>
        /// <param name="user">ユーザーオブジェクト</param>
        /// <param name="passwordHash">パスワード文字列（未暗号化?）</param>
        /// <returns>IdentityResult オブジェクト</returns>
        public Task SetPasswordHashAsync(User user, string passwordHash)
        {
            user.Password = passwordHash;
            return Task.FromResult(default(object));
        }

        /// <summary>
        /// ユーザーからパスワードのハッシュを取得する
        /// </summary>
        /// <param name="user">ユーザーオブジェクト</param>
        /// <returns>パスワードハッシュ文字列</returns>
        public Task<string> GetPasswordHashAsync(User user)
        {
            var passwordHash = db.Users.Find(user.Id).Password;
            return Task.FromResult(passwordHash);
        }

        /// <summary>
        /// パスワードが設定されている場合に true を返却します。
        /// </summary>
        /// <param name="user">ユーザーオブジェクト</param>
        /// <returns>パスワードが設定されている場合は true、それ以外の場合は false</returns>
        public Task<bool> HasPasswordAsync(User user)
        {
            return Task.FromResult(user.Password != null);
        }

        public void Dispose()
        {
            // プロパティをここで明示的に破棄
            // 参考 → https://aspnet.codeplex.com/SourceControl/latest#Samples/Identity/AspNet.Identity.MySQL/RoleStore.cs
            if (db != null)
            {
                db.Dispose();
                db = null;
            }
        }
    }
}