# MVC5IdentitySample
Identity をベースにした、ユーザー情報を DB に持つ独自の認証サンプルアプリです。

## 始め方
1. 本リモートリポジトリをクローンする。
1. ビルドを実行する。
1. Visual Studio パッケージマネージャーコンソールから、`Update-Database -Verbose` を実行する。
1. デバッグなしで開始 (Ctrl + F5) して、アプリの動作を確認する。

## 仕様
### 認証方法
- ユーザ名とパスワードでログイン

### データベースのテーブル
#### Users テーブル
- Id : String、必須、GUID を設定する。
- UserName : String、必須
- Password : String、必須
- Memo : String

### ページ一覧
本来であれば、ユーザ一覧・詳細・編集・削除はログイン必要とするべきです。
しかし、サンプルということであえてログイン不要でアクセス・操作可能としています。

- /Home/Index ← ログイン必要
- /Home/About ← ログイン必要
- /Home/Contact ← ログイン必要
- /Users/Login : ログイン
- /Users/Index : ユーザ一覧
- /Users/Detail : ユーザ詳細
- /Users/Create : ユーザ作成
- /Users/Edit : ユーザ編集
- /Users/ChangePasswrod : パスワード変更 ← ログイン必要
- /Users/Delete : ユーザ削除

### ページ遷移の補足
- ログオフ時は Home コントローラーページにアクセスできず、Users/Login ページにリダイレクトする。
- ログインページで認証すると、Home/Index へリダイレクトする。
- ユーザー作成と同時にログイン完了状態にする。
- ユーザー情報を更新してもログイン状態を継続する。
- パスワード変更は、専用のページを用意する。モデルビューも用意する（他のページは Users モデルを使い回す）。
- パスワード変更は、認証を必要とする。どのリンクから辿ったとしても、ログイン中ユーザのパスワードを変更する。
- ログインしているユーザーが自分自身を削除した場合は、ログオフする。

## 詳細
- [【ASP.NET MVC5】Identity 2 をベースにした、ユーザー情報を DB に持つ独自の認証プロジェクトのチュートリアル – oki2a24](http://oki2a24.com/2015/11/29/auth-based-on-identity-asp-net-mvc5/)
