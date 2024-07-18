export class LoginModel {
  email: string | undefined;
  password: string | undefined;
  twoFactorCode: string | undefined;
  twoFactorRecoveryCode: string | unknown;
}
