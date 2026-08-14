import { useEffect, useState } from "react";
import { useForm } from "react-hook-form";
import { useNavigate } from "react-router-dom";
import { authService } from "../services/authService";
import { userService } from "../../user/services/userService";
import { referenceDataService } from "../../../shared/services/referenceDataService";
import type { RefTerm } from "../../../shared/services/referenceDataService";
import { feedbackService } from "../../../shared/services/feedback/feedbackService";
import LapInput from "../../../shared/components/ui/LapInput/LapInput";
import LapButton from "../../../shared/components/ui/LapButton/LapButton";
import { registerFormStrings } from "../utils/constants";
import "../pages/Register/Register.css";

export interface RegisterFormData {
  fullName: string;
  email: string;
  password: string;
  mobileNumber: string;
  designationId: string;
  genderId: string;
}

export interface RegisterFormInitialData {
  id?: string;
  fullName: string;
  email: string;
  mobileNumber: string;
  designationId: string;
  genderId: string;
  roles?: string[];
}

interface RegisterFormProps {
  mode?: "create" | "edit";
  initialData?: RegisterFormInitialData | null;
  onSuccess?: () => void;
  onClose?: () => void;
}

const FALLBACK_DESIGNATIONS: RefTerm[] = [
  { id: "a1b2c3d4-e5f6-7890-abcd-ef1234567890", name: "Professor" },
  { id: "b2c3d4e5-f6a7-8901-bcde-f12345678901", name: "Researcher" },
  { id: "c3d4e5f6-a7b8-9012-cdef-123456789012", name: "Student" },
  { id: "d4e5f6a7-b8c9-0123-defa-234567890123", name: "Administrator" },
];

const FALLBACK_GENDERS: RefTerm[] = [
  { id: "e5f6a7b8-c9d0-1234-efab-345678901234", name: "Male" },
  { id: "f6a7b8c9-d0e1-2345-fabc-456789012345", name: "Female" },
  { id: "a7b8c9d0-e1f2-3456-abcd-567890123456", name: "Prefer not to say" },
];

export default function RegisterForm({
  mode = "create",
  initialData = null,
  onSuccess,
  onClose,
}: RegisterFormProps) {
  const [showPassword, setShowPassword] = useState(false);
  const [serverError, setServerError] = useState("");
  const [designations, setDesignations] = useState<RefTerm[]>(
    FALLBACK_DESIGNATIONS,
  );
  const [genders, setGenders] = useState<RefTerm[]>(FALLBACK_GENDERS);
  const navigate = useNavigate();
  const isEditMode = mode === "edit";

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors, isSubmitting },
  } = useForm<RegisterFormData>({
    defaultValues: initialData
      ? {
          fullName: initialData.fullName,
          email: initialData.email,
          password: "",
          mobileNumber: initialData.mobileNumber,
          designationId: initialData.designationId,
          genderId: initialData.genderId,
        }
      : undefined,
  });

  useEffect(() => {
    if (initialData) {
      reset({
        fullName: initialData.fullName,
        email: initialData.email,
        password: "",
        mobileNumber: initialData.mobileNumber,
        designationId: initialData.designationId,
        genderId: initialData.genderId,
      });
    }
  }, [initialData, reset, designations, genders]);

  useEffect(() => {
    Promise.all([
      referenceDataService.getDesignations().catch(() => FALLBACK_DESIGNATIONS),
      referenceDataService.getGenders().catch(() => FALLBACK_GENDERS),
    ]).then(([des, gen]) => {
      setDesignations(des);
      setGenders(gen);
    });
  }, []);

  const onSubmit = async (data: RegisterFormData) => {
    setServerError("");
    try {
      if (isEditMode && initialData?.id) {
        await userService.updateUser(initialData.id, {
          fullName: data.fullName,
          email: data.email,
          mobileNumber: data.mobileNumber,
          designationId: data.designationId,
          genderId: data.genderId,
          roles: initialData.roles ?? [],
        });
        if (onSuccess) onSuccess();
        if (onClose) onClose();
        feedbackService.showToast(
          registerFormStrings.success.updated,
          "success",
        );
      } else {
        await authService.register(data);
        if (onSuccess) {
          onSuccess();
        }
        if (onClose) {
          onClose();
        } else {
          navigate("/login");
        }
        feedbackService.showToast(
          registerFormStrings.success.created,
          "success",
        );
      }
    } catch {
      setServerError(
        isEditMode
          ? registerFormStrings.error.updateFailed
          : registerFormStrings.error.registrationFailed,
      );
    }
  };

  return (
    <form className="register-form" onSubmit={handleSubmit(onSubmit)}>
      <LapInput
        id="reg-fullName"
        label={registerFormStrings.labels.fullName}
        placeholder={registerFormStrings.placeholders.fullName}
        error={errors.fullName?.message}
        {...register("fullName", {
          required: registerFormStrings.validation.fullNameRequired,
        })}
      />

      <LapInput
        id="reg-email"
        label={registerFormStrings.labels.email}
        htmlType="email"
        placeholder={registerFormStrings.placeholders.email}
        error={errors.email?.message}
        {...register("email", {
          required: registerFormStrings.validation.emailRequired,
          pattern: {
            value: /^\S+@\S+$/i,
            message: registerFormStrings.validation.emailInvalid,
          },
        })}
      />

      <div className="register-row">
        {!isEditMode && (
          <LapInput
            id="reg-password"
            label={registerFormStrings.labels.password}
            htmlType={showPassword ? "text" : "password"}
            placeholder={registerFormStrings.placeholders.password}
            error={errors.password?.message}
            rightElement={
              <button
                type="button"
                className="register-password-toggle"
                onClick={() => setShowPassword(!showPassword)}
              >
                <span className="material-symbols-outlined register-password-toggle-icon">
                  {showPassword ? "visibility_off" : "visibility"}
                </span>
              </button>
            }
            {...register("password", {
              required: registerFormStrings.validation.passwordRequired,
              minLength: {
                value: 6,
                message: registerFormStrings.validation.passwordMinLength,
              },
            })}
          />
        )}

        <LapInput
          id="reg-mobileNumber"
          label={registerFormStrings.labels.mobileNumber}
          htmlType="tel"
          placeholder={registerFormStrings.placeholders.mobileNumber}
          error={errors.mobileNumber?.message}
          {...register("mobileNumber", {
            required: registerFormStrings.validation.mobileRequired,
            pattern: {
              value: /^\+?[\d\s-]{7,15}$/,
              message: registerFormStrings.validation.mobileInvalid,
            },
          })}
        />
      </div>

      <div className="register-row">
        <div className="register-field">
          <label className="register-label" htmlFor="reg-designationId">
            {registerFormStrings.labels.designation}
          </label>
          <select
            className="register-select"
            id="reg-designationId"
            {...register("designationId", {
              required: registerFormStrings.validation.designationRequired,
            })}
          >
            <option value="">
              {registerFormStrings.placeholders.selectDesignation}
            </option>
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
          <label className="register-label" htmlFor="reg-genderId">
            {registerFormStrings.labels.gender}
          </label>
          <select
            className="register-select"
            id="reg-genderId"
            {...register("genderId", {
              required: registerFormStrings.validation.genderRequired,
            })}
          >
            <option value="">
              {registerFormStrings.placeholders.selectGender}
            </option>
            {genders.map((g) => (
              <option key={g.id} value={g.id}>
                {g.name}
              </option>
            ))}
          </select>
          {errors.genderId && (
            <span className="register-error">{errors.genderId.message}</span>
          )}
        </div>
      </div>

      {serverError && (
        <div className="register-server-error">{serverError}</div>
      )}

      <div className="register-actions">
        {onClose && (
          <LapButton type="ghost" onClick={onClose}>
            {registerFormStrings.buttons.cancel}
          </LapButton>
        )}
        <LapButton
          type="primary"
          htmlType="submit"
          loading={isSubmitting}
          fullWidth
          icon={
            !isSubmitting ? (
              <span className="material-symbols-outlined register-submit-icon">
                {isEditMode
                  ? registerFormStrings.buttons.checkIcon
                  : registerFormStrings.buttons.arrowForwardIcon}
              </span>
            ) : undefined
          }
        >
          {isSubmitting
            ? isEditMode
              ? registerFormStrings.buttons.saving
              : registerFormStrings.buttons.creating
            : isEditMode
              ? registerFormStrings.buttons.saveChanges
              : registerFormStrings.buttons.createAccount}
        </LapButton>
      </div>
    </form>
  );
}
