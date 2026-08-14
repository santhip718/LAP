import { useState } from "react";
import { useForm } from "react-hook-form";
import { useNavigate } from "react-router-dom";
import { authService } from "@/features/auth/services/authService";
import { useAuth } from "@/core/providers/AuthProvider/useAuth";
import { feedbackService } from "@/shared/services/feedback";
import { getUserRoles, normalizeRole } from "@/features/auth/utils/authUtils";
import { USER_ROLES } from "@/shared/constants/roles";
import { ROUTES } from "@/shared/constants/routes";
import { REGEX } from "@/shared/constants/regex";
import { LOGIN_UI, LOGIN_VALIDATION } from "./Login.constants";
import LapInput from "@/shared/components/ui/LapInput/LapInput";
import LapButton from "@/shared/components/ui/LapButton/LapButton";
import Typography from "@mui/material/Typography";
import "./Login.css";

interface LoginForm {
  email: string;
  password: string;
}

export default function Login() {
  const [showPassword, setShowPassword] = useState(false);
  const [serverError, setServerError] = useState("");
  const navigate = useNavigate();
  const { checkAuth } = useAuth();
  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<LoginForm>();

  const onSubmit = async (data: LoginForm) => {
    setServerError("");
    try {
      await authService.login(data);
      checkAuth();
      feedbackService.showToast("Logged in successfully", "success");
      const roles = getUserRoles().map(normalizeRole);
      if (roles.includes(USER_ROLES.ADMIN)) {
        navigate(ROUTES.DASHBOARD, { replace: true });
      } else {
        navigate(ROUTES.DISCOVER, { replace: true });
      }
    } catch {
      setServerError("Invalid email or password. Please try again.");
    }
  };

  return (
    <div className="login-page">
      <main className="login-main">
        <div className="login-bg-glow">
          <div className="login-bg-glow-1" />
          <div className="login-bg-glow-2" />
        </div>

        <div className="login-card">
          <div className="login-card-header">
            <Typography
              variant="h2"
              className="login-card-title"
              component="h1"
            >
              {LOGIN_UI.TITLE}
            </Typography>
            <Typography
              variant="body1"
              className="login-card-subtitle"
              component="p"
            >
              {LOGIN_UI.SUBTITLE}
            </Typography>
          </div>

          <form className="login-form" onSubmit={handleSubmit(onSubmit)}>
            <LapInput
              label={LOGIN_UI.EMAIL_LABEL}
              id="email"
              htmlType="email"
              placeholder="name@institution.edu"
              error={errors.email?.message}
              {...register("email", {
                required: LOGIN_VALIDATION.EMAIL_REQUIRED,
                pattern: {
                  value: LOGIN_VALIDATION.EMAIL_PATTERN,
                  message: LOGIN_VALIDATION.EMAIL_INVALID,
                },
              })}
            />

            <LapInput
              label={LOGIN_UI.PASSWORD_LABEL}
              id="password"
              htmlType={showPassword ? "text" : "password"}
              placeholder="••••••••"
              error={errors.password?.message}
              {...register("password", {
                required: LOGIN_VALIDATION.PASSWORD_REQUIRED,
                minLength: {
                  value: LOGIN_VALIDATION.PASSWORD_MIN_LENGTH,
                  message: LOGIN_VALIDATION.PASSWORD_MIN_MESSAGE,
                },
                maxLength: {
                  value: LOGIN_VALIDATION.PASSWORD_MAX_LENGTH,
                  message: LOGIN_VALIDATION.PASSWORD_MAX_MESSAGE,
                },
                validate: {
                  hasUpper: (v) =>
                    REGEX.PASSWORD_UPPER.test(v) ||
                    LOGIN_VALIDATION.PASSWORD_UPPER_MESSAGE,
                  hasLower: (v) =>
                    REGEX.PASSWORD_LOWER.test(v) ||
                    LOGIN_VALIDATION.PASSWORD_LOWER_MESSAGE,
                  hasDigit: (v) =>
                    REGEX.PASSWORD_DIGIT.test(v) ||
                    LOGIN_VALIDATION.PASSWORD_DIGIT_MESSAGE,
                },
              })}
            />

            {serverError && (
              <div className="login-server-error">{serverError}</div>
            )}

            <LapButton
              htmlType="submit"
              disabled={isSubmitting}
              loading={isSubmitting}
              icon={
                <span className="material-symbols-outlined">
                  {LOGIN_UI.ARROW_FORWARD_ICON}
                </span>
              }
              fullWidth
              style={{ marginTop: 4 }}
            >
              {isSubmitting ? LOGIN_UI.SUBMITTING_TEXT : LOGIN_UI.SUBMIT_TEXT}
            </LapButton>
          </form>
        </div>
      </main>
    </div>
  );
}
