import { useState } from "react";
import { useForm } from "react-hook-form";
import { authService } from "../services/authService";
import { getUserRoles, normalizeRole } from "../utils/authHelpers";
import { useNavigate } from "react-router-dom";
import LapInput from "../../../shared/components/ui/LapInput/LapInput";
import LapButton from "../../../shared/components/ui/LapButton/LapButton";
import { loginStrings } from "../utils/constants";

interface LoginFormData {
  email: string;
  password: string;
}

export default function LoginForm() {
  const [showPassword, setShowPassword] = useState(false);
  const [serverError, setServerError] = useState("");
  const navigate = useNavigate();
  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<LoginFormData>();

  const onSubmit = async (data: LoginFormData) => {
    setServerError("");
    try {
      await authService.login(data);
      const roles = getUserRoles().map(normalizeRole);
      if (roles.includes("admin")) {
        navigate("/dashboard", { replace: true });
      } else {
        navigate("/profile", { replace: true });
      }
    } catch {
      setServerError(loginStrings.error.invalidCredentials);
    }
  };

  return (
    <form className="login-form" onSubmit={handleSubmit(onSubmit)}>
      <LapInput
        id="email"
        label={loginStrings.form.emailLabel}
        htmlType="email"
        placeholder={loginStrings.form.emailPlaceholder}
        error={errors.email?.message}
        {...register("email", {
          required: loginStrings.validation.emailRequired,
          pattern: { value: /^\S+@\S+$/i, message: loginStrings.validation.emailInvalid },
        })}
      />

      <div className="login-field">
        <div className="login-label-row">
          <label className="login-label" htmlFor="password">
            {loginStrings.form.passwordLabel}
          </label>
        </div>
        <div className="login-password-wrapper">
          <LapInput
            id="password"
            htmlType={showPassword ? "text" : "password"}
            placeholder={loginStrings.form.passwordPlaceholder}
            error={errors.password?.message}
            {...register("password", {
              required: loginStrings.validation.passwordRequired,
              minLength: { value: 6, message: loginStrings.validation.passwordMinLength },
            })}
          />
          <button
            className="login-password-toggle"
            type="button"
            onClick={() => setShowPassword(!showPassword)}
          >
            <span className="material-symbols-outlined login-password-toggle-icon">
              {showPassword ? "visibility_off" : "visibility"}
            </span>
          </button>
        </div>
      </div>

      {serverError && (
        <div className="login-server-error">{serverError}</div>
      )}

      <LapButton
        type="primary"
        htmlType="submit"
        loading={isSubmitting}
        fullWidth
        icon={!isSubmitting ? <span className="material-symbols-outlined login-submit-icon">{loginStrings.form.arrowForwardIcon}</span> : undefined}
      >
        {isSubmitting ? loginStrings.form.signingInText : loginStrings.form.signInText}
      </LapButton>
    </form>
  );
}
