import { useEffect, useState } from "react";
import { useForm } from "react-hook-form";
import { Link, useNavigate } from "react-router-dom";
import { authService } from "@/features/auth/services/authService";
import { referenceDataService } from "@/shared/services/referenceData";
import type { RefTerm } from "@/shared/services/referenceData";
import { REGEX } from "@/shared/constants/regex";
import LapInput from "@/shared/components/ui/LapInput/LapInput";
import LapButton from "@/shared/components/ui/LapButton/LapButton";
import Typography from "@mui/material/Typography";
import { ROUTES } from "@/shared/constants/routes";
import {
  REGISTER_UI,
  REGISTER_VALIDATION,
  REGISTER_ERROR,
} from "./Register.constants";
import "./Register.css";
 
interface RegisterForm {
  fullName: string;
  email: string;
  password: string;
  mobileNumber: string;
  designationId: string;
  genderId: string;
}
 
export default function Register() {
  const [showPassword, setShowPassword] = useState(false);
  const [serverError, setServerError] = useState("");
  const [designations, setDesignations] = useState<RefTerm[]>([]);
  const [genders, setGenders] = useState<RefTerm[]>([]);
  const navigate = useNavigate();
  const {
    register: registerField,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<RegisterForm>();
 
  useEffect(() => {
    Promise.all([
      referenceDataService.getDesignations().catch(() => []),
      referenceDataService.getGenders().catch(() => []),
    ]).then(([des, gen]) => {
      setDesignations(des);
      setGenders(gen);
    });
  }, []);
 
  const onSubmit = async (data: RegisterForm) => {
    setServerError("");
    try {
      await authService.register(data);
      navigate(ROUTES.LOGIN);
    } catch {
      setServerError(REGISTER_ERROR.FAILED);
    }
  };
 
  return (
    <div className="register-page">
      <header className="register-header">
        <Typography variant="h3" className="register-header-brand" component="div">
          {REGISTER_UI.BRAND}
        </Typography>
        <Link to={ROUTES.HOME} className="register-header-back">
          <span className="material-symbols-outlined register-header-back-icon">
            {REGISTER_UI.ARROW_BACK_ICON}
          </span>
          {REGISTER_UI.BACK_TO_HOME}
        </Link>
      </header>
 
      <main className="register-main">
        <div className="register-bg-glow">
          <div className="register-bg-glow-1" />
          <div className="register-bg-glow-2" />
        </div>
 
        <div className="register-card">
          <div className="register-card-header">
            <Typography variant="h2" className="register-card-title" component="h1">
              {REGISTER_UI.TITLE}
            </Typography>
            <Typography variant="body1" className="register-card-subtitle" component="p">
              {REGISTER_UI.SUBTITLE}
            </Typography>
          </div>
 
          <form className="register-form" onSubmit={handleSubmit(onSubmit)}>
            <LapInput
              label={REGISTER_UI.FULL_NAME_LABEL}
              id="fullName"
              placeholder={REGISTER_UI.FULL_NAME_PLACEHOLDER}
              error={errors.fullName?.message}
              {...registerField("fullName", {
                required: REGISTER_VALIDATION.FULL_NAME_REQUIRED,
                minLength: {
                  value: REGISTER_VALIDATION.FULL_NAME_MIN_LENGTH,
                  message: REGISTER_VALIDATION.FULL_NAME_MIN_MESSAGE,
                },
                maxLength: {
                  value: REGISTER_VALIDATION.FULL_NAME_MAX_LENGTH,
                  message: REGISTER_VALIDATION.FULL_NAME_MAX_MESSAGE,
                },
                pattern: {
                  value: REGISTER_VALIDATION.FULL_NAME_PATTERN,
                  message: REGISTER_VALIDATION.FULL_NAME_INVALID,
                },
              })}
            />
 
            <LapInput
              label={REGISTER_UI.EMAIL_LABEL}
              id="email"
              htmlType="email"
              placeholder={REGISTER_UI.EMAIL_PLACEHOLDER}
              error={errors.email?.message}
              {...registerField("email", {
                required: REGISTER_VALIDATION.EMAIL_REQUIRED,
                pattern: {
                  value: REGISTER_VALIDATION.EMAIL_PATTERN,
                  message: REGISTER_VALIDATION.EMAIL_INVALID,
                },
              })}
            />
 
            <div className="register-row">
              <LapInput
                label={REGISTER_UI.PASSWORD_LABEL}
                id="password"
                htmlType={showPassword ? "text" : "password"}
                placeholder={REGISTER_UI.PASSWORD_PLACEHOLDER}
                error={errors.password?.message}
                rightElement={
                  <LapButton
                    type="ghost"
                    htmlType="button"
                    onClick={() => setShowPassword(!showPassword)}
                    style={{
                      padding: 4,
                      borderRadius: 4,
                      color: "var(--outline)",
                    }}
                  >
                    <span
                      className="material-symbols-outlined"
                      style={{ fontSize: 20 }}
                    >
                      {showPassword
                        ? REGISTER_UI.VISIBILITY_OFF
                        : REGISTER_UI.VISIBILITY}
                    </span>
                  </LapButton>
                }
                {...registerField("password", {
                  required: REGISTER_VALIDATION.PASSWORD_REQUIRED,
                  minLength: {
                    value: REGISTER_VALIDATION.PASSWORD_MIN_LENGTH,
                    message: REGISTER_VALIDATION.PASSWORD_MIN_MESSAGE,
                  },
                  maxLength: {
                    value: REGISTER_VALIDATION.PASSWORD_MAX_LENGTH,
                    message: REGISTER_VALIDATION.PASSWORD_MAX_MESSAGE,
                  },
                  validate: {
                    hasUpper: (v) => REGEX.PASSWORD_UPPER.test(v) || REGISTER_VALIDATION.PASSWORD_UPPER_MESSAGE,
                    hasLower: (v) => REGEX.PASSWORD_LOWER.test(v) || REGISTER_VALIDATION.PASSWORD_LOWER_MESSAGE,
                    hasDigit: (v) => REGEX.PASSWORD_DIGIT.test(v) || REGISTER_VALIDATION.PASSWORD_DIGIT_MESSAGE,
                  },
                })}
              />
 
              <LapInput
                label={REGISTER_UI.MOBILE_LABEL}
                id="mobileNumber"
                htmlType="tel"
                placeholder={REGISTER_UI.MOBILE_PLACEHOLDER}
                error={errors.mobileNumber?.message}
                {...registerField("mobileNumber", {
                  required: REGISTER_VALIDATION.MOBILE_REQUIRED,
                  pattern: {
                    value: REGISTER_VALIDATION.MOBILE_PATTERN,
                    message: REGISTER_VALIDATION.MOBILE_INVALID,
                  },
                })}
              />
            </div>
 
            <div className="register-row">
              <div className="register-field">
                <label className="register-label" htmlFor="designationId">
                  {REGISTER_UI.DESIGNATION_LABEL}
                </label>
                <select
                  className="register-select"
                  id="designationId"
                  {...registerField("designationId", {
                    required: REGISTER_VALIDATION.DESIGNATION_REQUIRED,
                  })}
                >
                  <option value="">{REGISTER_UI.SELECT_DESIGNATION}</option>
                  {designations.map((d) => (
                    <option key={d.id} value={d.id}>
                      {d.name}
                    </option>
                  ))}
                </select>
                {errors.designationId && (
                  <span className="register-error">
                    {errors.designationId.message}
                  </span>
                )}
              </div>
 
              <div className="register-field">
                <label className="register-label" htmlFor="genderId">
                  {REGISTER_UI.GENDER_LABEL}
                </label>
                <select
                  className="register-select"
                  id="genderId"
                  {...registerField("genderId", {
                    required: REGISTER_VALIDATION.GENDER_REQUIRED,
                  })}
                >
                  <option value="">{REGISTER_UI.SELECT_GENDER}</option>
                  {genders.map((g) => (
                    <option key={g.id} value={g.id}>
                      {g.name}
                    </option>
                  ))}
                </select>
                {errors.genderId && (
                  <span className="register-error">
                    {errors.genderId.message}
                  </span>
                )}
              </div>
            </div>
 
            {serverError && (
              <div className="register-server-error">{serverError}</div>
            )}
 
            <LapButton
              htmlType="submit"
              disabled={isSubmitting}
              loading={isSubmitting}
              icon={
                <span className="material-symbols-outlined">
                  {REGISTER_UI.ARROW_FORWARD_ICON}
                </span>
              }
              fullWidth
            >
              {isSubmitting
                ? REGISTER_UI.SUBMITTING_TEXT
                : "submit"}
            </LapButton>
          </form>
 
          <div className="register-login">
            <p className="register-login-text">
              {REGISTER_UI.ALREADY_ACCOUNT}
              <Link to={ROUTES.LOGIN} className="register-login-link">
                {REGISTER_UI.SIGN_IN}
              </Link>
            </p>
          </div>
        </div>
      </main>
    </div>
  );
}
 
 