import { lazy, Suspense } from "react";
import { BrowserRouter, Routes, Route } from "react-router-dom";
import LapLayout from "@/shared/components/layout/LapLayout/LapLayout";
import ProtectedRoute from "@/core/routes/ProtectedRoute";
import PublicRoute from "@/core/routes/PublicRoute";
import LapFeedbackContainer from "@/shared/components/feedback/LapFeedbackContainer/LapFeedbackContainer";
import LapRouteErrorBoundary from "@/shared/components/feedback/LapErrorBoundary/LapRouteErrorBoundary";
import LapSpinnerv1 from "@/shared/components/ui/LapSpinnerv1/LapSpinnerv1";
import { USER_ROLES } from "@/shared/constants/roles";
import { ROUTES } from "@/shared/constants/routes";

const Home = lazy(() => import("@/features/home/pages/Home/Home"));
const Login = lazy(() => import("@/features/auth/pages/Login/Login"));
const Register = lazy(() => import("@/features/auth/pages/Register/Register"));
const AdminDashboard = lazy(() => import("@/features/admin/pages/AdminDashboard/AdminDashboard"));
const MyEnrollments = lazy(() => import("@/features/user/pages/MyEnrollments/MyEnrollments"));
const DiscoverCourses = lazy(() => import("@/features/user/pages/DiscoverCourses/DiscoverCourses"));
const CourseOverview = lazy(() => import("@/features/user/pages/CourseOverview/CourseOverview"));
const ViewCourseContent = lazy(() => import("@/features/user/pages/ViewCourseContent/ViewCourseContent"));
const AssessmentTest = lazy(() => import("@/features/user/pages/AssessmentTest/AssessmentTest"));
const UserProfile = lazy(() => import("@/features/user/pages/UserProfile/UserProfile"));
const AssessmentHistory = lazy(() => import("@/features/user/pages/AssessmentHistory/AssessmentHistory"));
const AssessmentResult = lazy(() => import("@/features/user/pages/AssessmentResult/AssessmentResult"));
const AssessmentManagement = lazy(() => import("@/features/admin/pages/AssessmentManagement/AssessmentManagement"));
const AdminAssessmentOverview = lazy(() => import("@/features/admin/pages/AssessmentOverview/AssessmentOverview"));
const StudentAssessmentOverview = lazy(() => import("@/features/user/pages/AssessmentOverview/AssessmentOverview"));
const LeaderboardPage = lazy(() => import("@/features/leaderboard/pages/LeaderboardPage"));
const CourseManagement = lazy(() => import("@/features/admin/pages/CourseManagement/CourseManagement"));
const AdminCourseOverview = lazy(() => import("@/features/admin/pages/CourseOverview/CourseOverview"));
const CreateAssessment = lazy(() => import("@/features/admin/pages/CreateAssessment/CreateAssessment"));
const EnrollmentManagement = lazy(() => import("@/features/admin/pages/EnrollmentManagement/EnrollmentManagement"));

function App() {
  return (
    <BrowserRouter>
      <LapFeedbackContainer />
      <LapRouteErrorBoundary>
        <Suspense fallback={<LapSpinnerv1 />}>
        <Routes>
          <Route element={<LapLayout />}>
            <Route
              path={ROUTES.HOME}
              element={
                <PublicRoute>
                  <Home />
                </PublicRoute>
              }
            />
            <Route
              path={ROUTES.LOGIN}
              element={
                <PublicRoute>
                  <Login />
                </PublicRoute>
              }
            />
            <Route
              path={ROUTES.DISCOVER}
              element={
                <ProtectedRoute allowedRoles={[USER_ROLES.STUDENT]}>
                  <DiscoverCourses />
                </ProtectedRoute>
              }
            />
            <Route
              path={ROUTES.MY_COURSES}
              element={
                <ProtectedRoute allowedRoles={[USER_ROLES.STUDENT]}>
                  <MyEnrollments />
                </ProtectedRoute>
              }
            />
            <Route
              path={ROUTES.PROFILE}
              element={
                <ProtectedRoute allowedRoles={[USER_ROLES.ADMIN, USER_ROLES.STUDENT]}>
                  <UserProfile />
                </ProtectedRoute>
              }
            />
            <Route
              path={ROUTES.ASSESSMENT_HISTORY}
              element={
                <ProtectedRoute allowedRoles={[USER_ROLES.STUDENT]}>
                  <AssessmentHistory />
                </ProtectedRoute>
              }
            />
            <Route
              path={ROUTES.COURSE_OVERVIEW}
              element={
                <ProtectedRoute allowedRoles={[USER_ROLES.STUDENT]}>
                  <CourseOverview />
                </ProtectedRoute>
              }
            />
            <Route
              path={ROUTES.COURSE_CONTENT}
              element={
                <ProtectedRoute allowedRoles={[USER_ROLES.STUDENT]}>
                  <ViewCourseContent />
                </ProtectedRoute>
              }
            />
            <Route
              path={ROUTES.ASSESSMENT_RESULT}
              element={
                <ProtectedRoute allowedRoles={[USER_ROLES.STUDENT]}>
                  <AssessmentResult />
                </ProtectedRoute>
              }
            />
            <Route
              path={ROUTES.DASHBOARD}
              element={
                <ProtectedRoute allowedRoles={[USER_ROLES.ADMIN]}>
                  <AdminDashboard />
                </ProtectedRoute>
              }
            />
            <Route
              path="/admin/dashboard"
              element={
                <ProtectedRoute allowedRoles={[USER_ROLES.ADMIN]}>
                  <AdminDashboard />
                </ProtectedRoute>
              }
            />
            <Route
              path="/admin/assessments"
              element={
                <ProtectedRoute allowedRoles={[USER_ROLES.ADMIN]}>
                  <AssessmentManagement />
                </ProtectedRoute>
              }
            />
            <Route
              path="/admin/assessments/:id"
              element={
                <ProtectedRoute allowedRoles={[USER_ROLES.ADMIN]}>
                  <AdminAssessmentOverview />
                </ProtectedRoute>
              }
            />
            <Route
              path={ROUTES.ADMIN_COURSES}
              element={
                <ProtectedRoute allowedRoles={[USER_ROLES.ADMIN]}>
                  <CourseManagement />
                </ProtectedRoute>
              }
            />
            <Route
              path="/admin/courses/:courseId"
              element={
                <ProtectedRoute allowedRoles={[USER_ROLES.ADMIN]}>
                  <AdminCourseOverview />
                </ProtectedRoute>
              }
            />
            <Route
              path="/admin/courses/:courseId/discussion"
              element={
                <ProtectedRoute allowedRoles={[USER_ROLES.ADMIN]}>
                  <AdminCourseOverview />
                </ProtectedRoute>
              }
            />
            <Route
              path="/admin/courses/:courseId/reviews"
              element={
                <ProtectedRoute allowedRoles={[USER_ROLES.ADMIN]}>
                  <AdminCourseOverview />
                </ProtectedRoute>
              }
            />
            <Route
              path="/admin/courses/:courseId/leaderboard"
              element={
                <ProtectedRoute allowedRoles={[USER_ROLES.ADMIN]}>
                  <AdminCourseOverview />
                </ProtectedRoute>
              }
            />
            <Route
              path="/admin/courses/:courseId/assessments/new"
              element={
                <ProtectedRoute allowedRoles={[USER_ROLES.ADMIN]}>
                  <CreateAssessment />
                </ProtectedRoute>
              }
            />
            <Route
              path={ROUTES.ADMIN_ENROLLMENTS}
              element={
                <ProtectedRoute allowedRoles={[USER_ROLES.ADMIN]}>
                  <EnrollmentManagement />
                </ProtectedRoute>
              }
            />
            <Route
              path={ROUTES.ASSESSMENT_OVERVIEW}
              element={
                <ProtectedRoute allowedRoles={[USER_ROLES.STUDENT]}>
                  <StudentAssessmentOverview />
                </ProtectedRoute>
              }
            />
            <Route
              path="/leaderboard"
              element={
                <ProtectedRoute allowedRoles={[USER_ROLES.ADMIN, USER_ROLES.STUDENT]}>
                  <LeaderboardPage />
                </ProtectedRoute>
              }
            />
          </Route>
          <Route
            path={ROUTES.REGISTER}
            element={
              <PublicRoute>
                <Register />
              </PublicRoute>
            }
          />
          <Route
            path={ROUTES.ASSESSMENT_TEST}
            element={
              <ProtectedRoute allowedRoles={[USER_ROLES.STUDENT]}>
                <AssessmentTest />
              </ProtectedRoute>
            }
          />
        </Routes>
        </Suspense>
      </LapRouteErrorBoundary>
    </BrowserRouter>
  );
}

export default App;
