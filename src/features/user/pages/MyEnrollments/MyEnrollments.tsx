import { useMemo, useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import Typography from "@mui/material/Typography";
import LapSpinnerv1 from "@/shared/components/ui/LapSpinnerv1/LapSpinnerv1";
import { useEnrollment } from "@/core/providers/EnrollmentProvider";
import { useCourse } from "@/core/providers/CourseProvider";
import type { EnrolledCourse } from "@/features/user/services/enrollmentService";
import { getRecommendedCourses } from "@/features/user/services/courseService";
import EnrolledCourseCard from "../../components/EnrolledCourseCard/EnrolledCourseCard";
import CourseCard from "../../components/CourseCard/CourseCard";
import type { Course } from "../../types/courseService.types";
import {
  PAGE_LABELS,
  EMPTY_ICON,
  BROWSE_ROUTE,
} from "./MyEnrollments.constants";
import "./MyEnrollments.css";

export default function MyEnrollments() {
  const { enrolledCourses, loading } = useEnrollment();
  const { courses: discoverCourses } = useCourse();
  const navigate = useNavigate();
  const [recommended, setRecommended] = useState<Course[]>([]);
  const [recommending, setRecommending] = useState(true);

  useEffect(() => {
    getRecommendedCourses()
      .then(setRecommended)
      .catch(() => {})
      .finally(() => setRecommending(false));
  }, []);

  const thumbnailByCourseId = useMemo(() => {
    const map = new Map<string, string>();
    discoverCourses.forEach((c) => {
      if (c.image) map.set(c.id, c.image);
    });
    return map;
  }, [discoverCourses]);

  const courses = useMemo(
    () =>
      Object.values(enrolledCourses).map(
        (c): EnrolledCourse => ({
          ...c,
          thumbnail: c.thumbnail || thumbnailByCourseId.get(c.courseId) || "",
        }),
      ),
    [enrolledCourses, thumbnailByCourseId],
  );

  return (
    <div className="my-enrollments">
      <main className="my-enrollments-main">
        <div className="my-enrollments-header">
          <Typography variant="h3">{PAGE_LABELS.TITLE}</Typography>
          <Typography variant="body1">
            {PAGE_LABELS.SUBTITLE}
          </Typography>
        </div>

        {recommended.length > 0 && (
          <section className="my-enrollments-recommended">
            <div className="my-enrollments-recommended-header">
              <div className="my-enrollments-recommended-header-text">
                <Typography variant="h5">{PAGE_LABELS.RECOMMENDED_TITLE}</Typography>
                <Typography variant="body2">
                  {PAGE_LABELS.RECOMMENDED_SUBTITLE}
                </Typography>
              </div>
            </div>
            <div className="my-enrollments-recommended-scroll">
              {recommended.map((course) => (
                <div
                  key={course.id}
                  className="my-enrollments-recommended-item"
                >
                  <CourseCard course={course} />
                </div>
              ))}
            </div>
          </section>
        )}

        {loading || recommending ? (
          <div className="my-enrollments-loading">
            <LapSpinnerv1 />
          </div>
        ) : courses.length === 0 ? (
          <div className="my-enrollments-empty">
            <span className="material-symbols-outlined my-enrollments-empty-icon">
              {EMPTY_ICON}
            </span>
            <Typography variant="h4">{PAGE_LABELS.EMPTY_TITLE}</Typography>
            <Typography variant="body1">
              {PAGE_LABELS.EMPTY_MESSAGE}
            </Typography>
            <button
              className="my-enrollments-browse"
              onClick={() => navigate(BROWSE_ROUTE)}
            >
              {PAGE_LABELS.BROWSE_BUTTON}
            </button>
          </div>
        ) : (
          <>
            <div className="my-enrollments-courses-header">
              <Typography variant="h5">{PAGE_LABELS.YOUR_COURSES}</Typography>
            </div>
            <div className="my-enrollments-grid">
              {courses.map((course) => (
                <EnrolledCourseCard key={course.id} course={course} />
              ))}
            </div>
          </>
        )}
      </main>
    </div>
  );
}
