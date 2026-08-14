import { useCallback, useMemo, useState } from "react";
import { useUserList } from "../../../user/hooks/useUserList";
import { userService } from "../../../user/services/userService";
import type { UserDetail, UserListItem } from "../../../user/types";
import ViewUserModal from "../../../user/components/ViewUserModal/ViewUserModal";
import EditUserModal from "../../../user/components/EditUserModal/EditUserModal";
import CreateUserModal from "../../../user/components/CreateUserModal/CreateUserModal";
import DeleteUserConfirm from "../../../user/components/DeleteUserConfirm/DeleteUserConfirm";
import LapDataTable, { type Column } from "../../../../shared/components/ui/LapDataTable/LapDataTable";
import { feedbackService } from "../../../../shared/services/feedback/feedbackService";
import { dashboardStrings } from "../../utils/constants";
import Typography from "@mui/material/Typography";
import "./AdminDashboard.css";

export default function AdminDashboard() {
  const {
    users: allUsers,
    loading,
    error,
    refresh,
    loadMore,
    hasMore,
  } = useUserList();

  const [searchFilter, setSearchFilter] = useState("");

  const users = useMemo(() => {
    if (!searchFilter.trim()) return allUsers;
    const q = searchFilter.trim().toLowerCase();
    return allUsers.filter(
      (u) =>
        u.fullName.toLowerCase().includes(q) ||
        u.email.toLowerCase().includes(q) ||
        u.roles.some((r) => r.toLowerCase().includes(q)),
    );
  }, [allUsers, searchFilter]);

  const [createOpen, setCreateOpen] = useState(false);
  const [viewOpen, setViewOpen] = useState(false);
  const [viewUser, setViewUser] = useState<UserDetail | null>(null);
  const [editOpen, setEditOpen] = useState(false);
  const [editUser, setEditUser] = useState<UserDetail | null>(null);
  const [deleteOpen, setDeleteOpen] = useState(false);
  const [deleteUserId, setDeleteUserId] = useState<string | null>(null);
  const [deleteUserName, setDeleteUserName] = useState("");
  const [loadingDetail, setLoadingDetail] = useState(false);

  const fetchDetail = useCallback(async (userId: string): Promise<UserDetail | null> => {
    setLoadingDetail(true);
    try {
      return await userService.getUserDetail(userId);
    } catch {
      feedbackService.showToast(dashboardStrings.error.loadDetailFailed, "error");
      return null;
    } finally {
      setLoadingDetail(false);
    }
  }, []);

  const handleAdd = () => {
    setCreateOpen(true);
  };

  const handleEdit = useCallback(async (userId: string) => {
    const user = await fetchDetail(userId);
    if (user) {
      setEditUser(user);
      setEditOpen(true);
    }
  }, [fetchDetail]);

  const handleView = useCallback(async (userId: string) => {
    const user = await fetchDetail(userId);
    if (user) {
      setViewUser(user);
      setViewOpen(true);
    }
  }, [fetchDetail]);

  const handleDelete = useCallback((userId: string, userName: string) => {
    setDeleteUserId(userId);
    setDeleteUserName(userName);
    setDeleteOpen(true);
  }, []);

  const columns = useMemo<Column<UserListItem>[]>(
    () => [
      {
        key: "fullName",
        label: dashboardStrings.columns.user,
        sortable: true,
        render: (_: unknown, row: UserListItem) => (
          <div className="admin-user-cell">
            <span className="material-symbols-outlined admin-user-icon">person</span>
            <span className="admin-user-name">{row.fullName}</span>
          </div>
        ),
      },
      {
        key: "email",
        label: dashboardStrings.columns.email,
        sortable: true,
        render: (_: unknown, row: UserListItem) => (
          <span className="admin-td-email">{row.email}</span>
        ),
      },
      {
        key: "roles",
        label: dashboardStrings.columns.roles,
        render: (_: unknown, row: UserListItem) => (
          <div className="admin-roles">
            {row.roles.map((role) => (
              <span key={role} className="admin-role-badge">{role}</span>
            ))}
          </div>
        ),
      },
      {
        key: "actions",
        label: "Actions",
        className: "cm-cell-center",
        thClassName: "cm-cell-center",
        render: (_: unknown, row: UserListItem) => (
          <div className="admin-actions-cell">
            <button
              className="admin-action-btn"
              type="button"
              aria-label={dashboardStrings.ariaLabels.viewUser}
              onClick={() => handleView(row.id)}
              disabled={loadingDetail}
            >
              <span className="material-symbols-outlined">visibility</span>
            </button>
            <button
              className="admin-action-btn"
              type="button"
              aria-label={dashboardStrings.ariaLabels.editUser}
              onClick={() => handleEdit(row.id)}
              disabled={loadingDetail}
            >
              <span className="material-symbols-outlined">edit</span>
            </button>
            <button
              className="admin-action-btn admin-action-delete"
              type="button"
              aria-label={dashboardStrings.ariaLabels.deleteUser}
              onClick={() => handleDelete(row.id, row.fullName)}
              disabled={loadingDetail}
            >
              <span className="material-symbols-outlined">delete</span>
            </button>
          </div>
        ),
      },
    ],
    [handleView, handleEdit, handleDelete, loadingDetail],
  );

  return (
    <div className="admin-dashboard">
      <main className="admin-dashboard-main">
        <div className="admin-dashboard-header">
          <div>
            <Typography variant="h2" className="admin-dashboard-h1">{dashboardStrings.pageTitle}</Typography>
            <Typography variant="body1" className="admin-dashboard-p">{dashboardStrings.pageSubtitle}</Typography>
          </div>
          <button className="admin-add-btn" type="button" onClick={handleAdd}>
            <span className="material-symbols-outlined">person_add</span>
            {dashboardStrings.addUserButton}
          </button>
        </div>

        <div className="admin-table-card">
          <div className="admin-table-header">
            <Typography variant="h5" className="admin-table-title">{dashboardStrings.table.title}</Typography>
            <div className="admin-table-actions">
              <label className="admin-search">
                <span className="material-symbols-outlined">search</span>
                <input
                  type="search"
                  value={searchFilter}
                  onChange={(e) => setSearchFilter(e.target.value)}
                  placeholder={dashboardStrings.table.searchPlaceholder}
                />
              </label>
              <button className="admin-icon-btn" type="button" onClick={refresh}>
                <span className="material-symbols-outlined">refresh</span>
              </button>
            </div>
          </div>

          {error && (
            <div className="admin-error-state">
              <span className="material-symbols-outlined">error</span>
              <span>{error}</span>
              <button type="button" onClick={refresh}>{dashboardStrings.table.errorRetry}</button>
            </div>
          )}

          {loading ? (
            <div className="admin-loading-state">
              <span className="material-symbols-outlined">progress_activity</span>
              <span>{dashboardStrings.table.loading}</span>
            </div>
          ) : (
            <LapDataTable<UserListItem>
              columns={columns}
              data={users}
              enableInfiniteScroll
              onLoadMore={loadMore}
              hasMore={hasMore}
            />
          )}
        </div>
      </main>

      <CreateUserModal
        open={createOpen}
        onClose={() => setCreateOpen(false)}
        onSuccess={refresh}
      />

      <ViewUserModal
        open={viewOpen}
        onClose={() => { setViewUser(null); setViewOpen(false); }}
        user={viewUser}
      />

      <EditUserModal
        open={editOpen}
        onClose={() => { setEditUser(null); setEditOpen(false); }}
        onSuccess={refresh}
        user={editUser}
      />

      <DeleteUserConfirm
        open={deleteOpen}
        onClose={() => setDeleteOpen(false)}
        onSuccess={refresh}
        userId={deleteUserId}
        userName={deleteUserName}
      />
    </div>
  );
}
